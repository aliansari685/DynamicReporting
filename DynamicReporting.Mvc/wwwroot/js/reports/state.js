export const {
    selectedReportId,
    metadata: Metadata,
    urls
} = window.reportPage;

export let currentPage = 1;
export let totalPages = 1;

export let activeFilters = [];

export let sortColumn = "";
export let sortDirection = "asc";

export function setCurrentPage(value) {
    currentPage = value;
}

export function setTotalPages(value) {
    totalPages = value;
}

export function setActiveFilters(value) {
    activeFilters = Array.isArray(value)
        ? value
        : [];
}

export function addFilter(filter) {
    activeFilters.push(filter);
}

export function removeFilter(index) {
    activeFilters.splice(index, 1);
}

export function clearFilters() {
    activeFilters = [];
}

export function setSorting(column, direction = "asc") {
    sortColumn = column || "";
    sortDirection = direction || "asc";
}

export function clearSorting() {
    sortColumn = "";
    sortDirection = "asc";
}