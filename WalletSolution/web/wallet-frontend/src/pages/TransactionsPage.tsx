import { useEffect, useState } from "react";
import { Box, Card, CardContent, Stack, Button, Typography } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import { getTransactions } from "../api/walletApi";

type TxRow = {
  id: string;
  type: string;
  amount: number;
  balanceAfter: number;
  description?: string | null;
  externalReference?: string | null;
  createdAt: string;
};

const columns: GridColDef<TxRow>[] = [
  { field: "type", headerName: "Type", width: 120 },
  { field: "amount", headerName: "Amount", width: 110 },
  { field: "balanceAfter", headerName: "Balance After", width: 150 },
  { field: "description", headerName: "Description", width: 300, flex: 1 },
  { field: "externalReference", headerName: "External Ref", width: 160 },
  {
    field: "createdAt",
    headerName: "Created At",
    width: 200,
    // use renderCell to format the date
    renderCell: (params) => {
      const v = params.value as string | undefined;
      return <span>{v ? dayjs(v).format("YYYY-MM-DD HH:mm:ss") : ""}</span>;
    },
  },
];

export default function TransactionsPage() {
  const storedUserId = localStorage.getItem("userId") ?? "";
  const [rows, setRows] = useState<TxRow[]>([]);
  const [total, setTotal] = useState<number | null>(null);
  const [loading, setLoading] = useState<boolean>(false);

  // paging state for DataGrid (we will use simple server paging controls)
  const [skip, setSkip] = useState<number>(0);
  const [take, setTake] = useState<number>(20);

  useEffect(() => {
    // auto-load when component mounts if we have a token
    if (!localStorage.getItem("jwt")) return;
    load(skip, take);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  async function load(currentSkip = 0, currentTake = take) {
    // client should be authenticated (token in localStorage) because backend reads user from JWT
    if (!localStorage.getItem("jwt")) {
      alert("You must be logged in to view transactions.");
      return;
    }

    setLoading(true);
    try {
      const res = await getTransactions(currentSkip, currentTake);
      setRows(res.items ?? []);
      setTotal(res.total ?? null);
      setSkip(currentSkip);
      setTake(currentTake);
    } catch (err: any) {
      console.error("getTransactions error", err);
      alert(err?.response?.data?.message ?? "Failed to load transactions");
      setRows([]);
      setTotal(null);
    } finally {
      setLoading(false);
    }
  }

  // DataGrid pagination handler (client-side handling to request server pages)
  const handlePageChange = (page: number, details?: any) => {
    // DataGrid page index starts at 0
    const newSkip = page * take;
    load(newSkip, take);
  };

  const handlePageSizeChange = (newPageSize: number) => {
    // switch to page 0 when page size changes
    load(0, newPageSize);
  };

  return (
    <Box sx={{ maxWidth: 1100, margin: "24px auto", padding: 2 }}>
      <Card>
        <CardContent>
          <Stack direction="row" gap={2} mb={2} alignItems="center">
            <Typography variant="subtitle1" sx={{ minWidth: 140 }}>
              User GUID:
            </Typography>
            <Typography variant="body2" sx={{ wordBreak: "break-all" }}>
              {storedUserId || "No userId in localStorage (login first)"}
            </Typography>

            <Box sx={{ flex: 1 }} />

            <Button variant="contained" onClick={() => load(0, take)} disabled={loading}>
              {loading ? "Loading..." : "Load"}
            </Button>
          </Stack>

          <div style={{ height: 600, width: "100%" }}>
            <DataGrid
              rows={rows}
              columns={columns}
              loading={loading}
              getRowId={(row: TxRow) => row.id}
              initialState={{
                pagination: { paginationModel: { pageSize: Math.max(10, take), page: Math.floor(skip / take) } },
              }}
              pageSizeOptions={[10, 20, 50, 100]}
              paginationMode="server"
              rowCount={total ?? undefined}
              onPaginationModelChange={(model) => {
                // model has shape { pageSize, page }
                handlePageChange(model.page);
                if (model.pageSize !== take) {
                  handlePageSizeChange(model.pageSize);
                }
              }}
              disableRowSelectionOnClick
            />
          </div>

          {total !== null && (
            <Box mt={2}>
              <strong>Total:</strong> {total}
            </Box>
          )}
        </CardContent>
      </Card>
    </Box>
  );
}
