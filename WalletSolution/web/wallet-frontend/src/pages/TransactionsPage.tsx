// src/pages/TransactionsPage.tsx
import { useState } from "react";
import { Box, TextField, Button, Card, CardContent, Stack } from "@mui/material";
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
    valueGetter: (value, row) => dayjs(row.createdAt).format("YYYY-MM-DD HH:mm:ss"),
  },
];

export default function TransactionsPage() {
  const [userId, setUserId] = useState<string>("");
  const [rows, setRows] = useState<TxRow[]>([]);
  const [total, setTotal] = useState<number | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [skip, setSkip] = useState<number>(0);
  const [take, setTake] = useState<number>(20);

  async function load() {
    if (!userId) return alert("Enter user GUID");
    setLoading(true);
    try {
      const res = await getTransactions(userId, skip, take);
      setRows(res.items ?? []);
      setTotal(res.total ?? null);
    } catch (err: any) {
      console.error("getTransactions error", err);
      alert(err?.response?.data?.message ?? "Failed to load transactions");
      setRows([]);
      setTotal(null);
    } finally {
      setLoading(false);
    }
  }

  return (
    <Box sx={{ maxWidth: 1100, margin: "24px auto", padding: 2 }}>
      <Card>
        <CardContent>
          <Stack direction="row" gap={2} mb={2}>
            <TextField
              label="User GUID"
              placeholder="aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"
              value={userId}
              onChange={(e) => setUserId(e.target.value)}
              sx={{ flex: 1 }}
            />
            <Button variant="contained" onClick={load} disabled={!userId || loading}>
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
                pagination: { paginationModel: { pageSize: Math.max(10, take), page: 0 } },
              }}
              pageSizeOptions={[10, 20, 50, 100]}
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
