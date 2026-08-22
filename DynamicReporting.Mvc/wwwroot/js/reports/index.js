import {
    selectedReportId
} from "./state.js";

import {
    loadReportData,
    changePage
} from "./report-data.js";

import {
    loadFilterMetadata,
    openFilterPanel,
    renderActiveFilterSummary
} from "./filters.js";

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

window.openFilterPanel = openFilterPanel;

document.addEventListener(
    "DOMContentLoaded",
    async function () {

        await startSignalR();

        if (selectedReportId) {

            await loadFilterMetadata();

            await loadReportData(1);

            renderActiveFilterSummary();

        }

    });
