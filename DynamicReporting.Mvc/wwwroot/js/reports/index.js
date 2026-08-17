console.log("REPORT INDEX JS LOADED");
import { selectedReportId } from "./state.js";
import { loadReportData, changePage } from "./report-data.js";
import { loadFilterableColumns } from "./filters.js";
import {
    openCreateModal,
    openEditModal,
    renderColumns,
    updateSelectedCount
} from "./editor.js";
import { deleteReport } from "./delete.js";
import {
    exportReport,
    openGeneratedReports,
    deleteGeneratedReport
} from "./generated-reports.js";
import { startSignalR } from "./signalr.js";

window.loadReportData = loadReportData;
window.changePage = changePage;
window.openCreateModal = openCreateModal;
window.openEditModal = openEditModal;
window.renderColumns = renderColumns;
window.updateSelectedCount = updateSelectedCount;
window.deleteReport = deleteReport;
window.exportReport = exportReport;
window.openGeneratedReports = openGeneratedReports;
window.deleteGeneratedReport = deleteGeneratedReport;

document.addEventListener(
    "DOMContentLoaded",
    async function () {

        if (selectedReportId) {

            await loadReportData(1);
            await loadFilterableColumns();

        }

        await startSignalR();

    });
