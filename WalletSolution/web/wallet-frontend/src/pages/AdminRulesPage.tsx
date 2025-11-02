import { useEffect, useState } from "react";
import {
    Box,
    Card,
    CardContent,
    Typography,
    Button,
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    TextField,
    Select,
    MenuItem,
    FormControl,
    InputLabel,
    IconButton,
} from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import { DataGrid, type GridColDef } from "@mui/x-data-grid";
import {
    getRules,
    createRule,
    updateRule,
    deleteRule,
    type ConfigurationRuleDto,
} from "../api/adminApi";

export default function AdminRulesPage() {
    const [rules, setRules] = useState<ConfigurationRuleDto[]>([]);
    const [loading, setLoading] = useState(false);
    const [open, setOpen] = useState(false);
    const [editing, setEditing] = useState<ConfigurationRuleDto | null>(null);

    const [form, setForm] = useState({
        serviceId: "",
        ruleType: "EARNING",
        pointsPerBaseAmount: 1,
        baseAmount: 100,
        isDefault: false,
    });

    async function load() {
        setLoading(true);
        try {
            const res = await getRules();
            setRules(res);
        } catch (err) {
            console.error(err);
            alert("Failed to load rules");
        } finally {
            setLoading(false);
        }
    }

    useEffect(() => {
        load();
    }, []);

    function openCreate() {
        setEditing(null);
        setForm({
            serviceId: "",
            ruleType: "EARNING",
            pointsPerBaseAmount: 1,
            baseAmount: 100,
            isDefault: false,
        });
        setOpen(true);
    }

    function openEdit(r: ConfigurationRuleDto) {
        setEditing(r);
        setForm({
            serviceId: r.serviceId,
            ruleType: r.ruleType,
            pointsPerBaseAmount: r.pointsPerBaseAmount,
            baseAmount: Number((r as any).baseAmount ?? 100),
            isDefault: r.isDefault ?? false,
        });
        setOpen(true);
    }

    async function save() {
        try {
            if (editing) {
                await updateRule(editing.id, form);
                setOpen(false);
            } else {
                await createRule(form);
                setOpen(false);
            }
            await load();
        } catch (err) {
            console.error(err);
            alert("Save failed");
        }
    }

    async function remove(id: string) {
        if (!confirm("Are you sure delete this rule?")) return;
        try {
            await deleteRule(id);
            await load();
        } catch (err) {
            console.error(err);
            alert("Delete failed");
        }
    }

    const columns: GridColDef<ConfigurationRuleDto>[] = [
        // { field: "id", headerName: "Id", width: 220 },
        { field: "serviceId", headerName: "ServiceId", width: 200 },
        { field: "ruleType", headerName: "Type", width: 120 },
        { field: "pointsPerBaseAmount", headerName: "Points Per Base", width: 150 },
        { field: "baseAmount", headerName: "Base Amount", width: 140 },
        {
            field: "isDefault",
            headerName: "Default",
            width: 100,
            valueFormatter: (params: any) => (params?.value ? "Yes" : "No"),
        },
        {
            field: "actions",
            headerName: "Actions",
            width: 140,
            sortable: false,
            filterable: false,
            renderCell: (params: any) => {
                const row = params.row as ConfigurationRuleDto;
                return (
                    <>
                        <IconButton size="small" onClick={() => openEdit(row)}>
                            <EditIcon />
                        </IconButton>
                        <IconButton size="small" onClick={() => remove(row.id)}>
                            <DeleteIcon />
                        </IconButton>
                    </>
                );
            },
        },
    ];

    return (
        <Box sx={{ maxWidth: 1100, margin: "24px auto", padding: 2 }}>
            <Card>
                <CardContent>
                    <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
                        <Typography variant="h6">Admin — Rules</Typography>
                        <Button variant="contained" onClick={openCreate}>
                            Create Rule
                        </Button>
                    </Box>

                    <div style={{ height: 420, width: "100%" }}>
                        <DataGrid
                            rows={rules}
                            columns={columns}
                            getRowId={(r: any) => r.id}
                            loading={loading}
                            pageSizeOptions={[5, 10, 20]}
                            initialState={{ pagination: { paginationModel: { pageSize: 10, page: 0 } } }}
                        />
                    </div>

                    <Dialog open={open} onClose={() => setOpen(false)} fullWidth>
                        <DialogTitle>{editing ? "Edit Rule" : "Create Rule"}</DialogTitle>
                        <DialogContent>
                            <Box display="grid" gridTemplateColumns="1fr 1fr" gap={2} mt={1}>
                                <TextField
                                    label="ServiceId"
                                    value={form.serviceId}
                                    onChange={(e) => setForm({ ...form, serviceId: e.target.value })}
                                />

                                <FormControl>
                                    <InputLabel>Type</InputLabel>
                                    <Select
                                        value={form.ruleType}
                                        label="Type"
                                        onChange={(e) => setForm({ ...form, ruleType: e.target.value as string })}
                                    >
                                        <MenuItem value="EARNING">EARNING</MenuItem>
                                        <MenuItem value="BURN">BURN</MenuItem>
                                    </Select>
                                </FormControl>

                                <TextField
                                    label="Points Per Base"
                                    type="number"
                                    value={form.pointsPerBaseAmount}
                                    onChange={(e) => setForm({ ...form, pointsPerBaseAmount: Number(e.target.value) })}
                                />

                                <TextField
                                    label="Base Amount"
                                    type="number"
                                    value={form.baseAmount}
                                    onChange={(e) => setForm({ ...form, baseAmount: Number(e.target.value) })}
                                />

                                <FormControl>
                                    <InputLabel>Default</InputLabel>
                                    <Select
                                        value={form.isDefault ? "true" : "false"}
                                        onChange={(e) => setForm({ ...form, isDefault: e.target.value === "true" })}
                                    >
                                        <MenuItem value="false">No</MenuItem>
                                        <MenuItem value="true">Yes</MenuItem>
                                    </Select>
                                </FormControl>
                            </Box>
                        </DialogContent>
                        <DialogActions>
                            <Button onClick={() => setOpen(false)}>Cancel</Button>
                            <Button variant="contained" onClick={save}>
                                Save
                            </Button>
                        </DialogActions>
                    </Dialog>
                </CardContent>
            </Card>
        </Box>
    );
}
