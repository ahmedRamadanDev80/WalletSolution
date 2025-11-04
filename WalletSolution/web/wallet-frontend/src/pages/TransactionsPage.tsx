// src/pages/TransactionsPage.tsx
import { useEffect, useState, useCallback } from "react";
import { Box, Card, CardContent, Stack, Button, Typography } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import type { GridColDef, GridPaginationModel } from "@mui/x-data-grid";
import dayjs from "dayjs";
import { getTransactions, type WalletTransactionDto, type TransactionsPaged } from "../api/walletApi";

const STORED_USER_ID = localStorage.getItem("userId") ?? "";

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
    renderCell: (params) => {
      const v = params.value as string | undefined;
      return <span>{v ? dayjs(v).format("YYYY-MM-DD HH:mm:ss") : ""}</span>;
    },
  },
];

export default function TransactionsPage() {
  const [rows, setRows] = useState<TxRow[]>([]);
  const [total, setTotal] = useState<number>(0);
  const [loading, setLoading] = useState<boolean>(false);

  // controlled pagination state
  const [page, setPage] = useState<number>(0); // zero-based
  const [pageSize, setPageSize] = useState<number>(20);

  const parseNumber = (v: number | string | undefined | null, fallback = 0) => {
    if (typeof v === "number") return v;
    if (typeof v === "string") {
      const n = Number(v);
      return Number.isNaN(n) ? fallback : n;
    }
    return fallback;
  };

  // load is defined with no external deps (clean solution)
  const load = useCallback(async (pageIndex: number, size: number) => {
    const mapDtoToRow = (r: WalletTransactionDto, idx: number, skip: number): TxRow => {
      const rawId = r.id ?? `${skip}-${idx}`;
      return {
        id: String(rawId),
        type: r.type ?? "",
        amount: parseNumber(r.amount, 0),
        balanceAfter: parseNumber(r.balanceAfter, 0),
        description: r.description ?? null,
        externalReference: r.externalReference ?? null,
        createdAt: r.createdAt ?? "",
      };
    };

    if (!localStorage.getItem("jwt")) {
      alert("You must be logged in to view transactions.");
      return;
    }

    setLoading(true);
    try {
      const skip = pageIndex * size;
      const take = size;

      const res: TransactionsPaged = await getTransactions(skip, take);

      const items = Array.isArray(res?.items) ? res.items : [];

      const safeRows = items.map((r, idx) => mapDtoToRow(r, idx, skip));
      setRows(safeRows);
      setTotal(typeof res?.total === "number" ? res.total : safeRows.length);
      setPage(pageIndex);
      setPageSize(size);
    } catch (err) {
      console.error("getTransactions error", err);
      const message = err instanceof Error ? err.message : "Failed to load transactions";
      alert(message);
      setRows([]);
      setTotal(0);
    } finally {
      setLoading(false);
    }
  }, []); // empty deps — clean

  useEffect(() => {
    if (!localStorage.getItem("jwt")) return;
    // initial load uses current pageSize state
    load(0, pageSize);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []); // run once on mount

  const paginationModel: GridPaginationModel = { page, pageSize };

  return (
    <Box sx={{ maxWidth: 1100, margin: "24px auto", padding: 2 }}>
      <Card>
        <CardContent>
          <Stack direction="row" gap={2} mb={2} alignItems="center">
            <Typography variant="subtitle1" sx={{ minWidth: 140 }}>
              User GUID:
            </Typography>
            <Typography variant="body2" sx={{ wordBreak: "break-all" }}>
              {STORED_USER_ID || "No userId in localStorage (login first)"}
            </Typography>

            <Box sx={{ flex: 1 }} />

            <Button variant="contained" onClick={() => load(0, pageSize)} disabled={loading}>
              {loading ? "Loading..." : "Load"}
            </Button>
          </Stack>

          <div style={{ height: 600, width: "100%" }}>
            <DataGrid
              rows={rows}
              columns={columns}
              loading={loading}
              getRowId={(row: TxRow) => row.id}
              pagination
              paginationMode="server"
              pageSizeOptions={[10, 20, 50, 100]}
              rowCount={total}
              paginationModel={paginationModel}
              onPaginationModelChange={(model: GridPaginationModel) => {
                if (model.pageSize !== pageSize) {
                  // convention: when pageSize changes reset to page 0
                  load(0, model.pageSize);
                } else if (model.page !== page) {
                  load(model.page, model.pageSize);
                }
              }}
              disableRowSelectionOnClick
            />
          </div>

          <Box mt={2}>
            <strong>Total:</strong> {total}
          </Box>
        </CardContent>
      </Card>
    </Box>
  );
}
