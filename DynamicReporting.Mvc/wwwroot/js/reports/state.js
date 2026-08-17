export const { selectedReportId, metadata: Metadata, urls } = window.reportPage;

export let currentPage = 1;
export let totalPages = 1;

export function setCurrentPage(value) {
    currentPage = value;
}

export function setTotalPages(value) {
    totalPages = value;
}
