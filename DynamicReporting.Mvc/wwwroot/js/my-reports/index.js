import {
    openEditModal,
    openCreateModal,
    copyReport
} from "../reports/editor.js";


document.addEventListener(
    "DOMContentLoaded",
    () => {

        /*
         * ==========================================
         * Create
         * ==========================================
         */

        const createButton =
            document.getElementById(
                "createReportButton");


        createButton?.addEventListener(
            "click",
            () => {
           openCreateModal();
           });


        /*
         * ==========================================
         * Edit
         * ==========================================
         */

        const editButtons =
            document.querySelectorAll(
                ".edit-report-btn");

        editButtons.forEach(
            button => {

                const id =
                    button.dataset.reportId;


                button.addEventListener(
                    "click",
                    () => {

                        openEditModal(id);

                    });

            });


        /*
         * ==========================================
         * Copy
         * ==========================================
         */

        const copyButtons =
            document.querySelectorAll(
                ".copy-report-btn");


        copyButtons.forEach(
            button => {
                button.addEventListener(
                    "click",
                    async function () {

                        const id =
                            this.dataset.reportId;

                        await copyReport(Number(id));

                    });

            });


        /*
         * ==========================================
         * Search
         * ==========================================
         */

        const searchInput =
            document.getElementById(
                "reportSearch");

        const cards =
            document.querySelectorAll(
                ".report-card");

        const emptySearch =
            document.getElementById(
                "emptySearch");


        if (searchInput) {

            searchInput.addEventListener(
                "input",
                function () {

                    const search =
                        this.value
                            .trim()
                            .toLocaleLowerCase("fa");


                    let visibleCount = 0;


                    cards.forEach(
                        card => {

                            const name =
                                (
                                    card.dataset.reportName
                                    ?? ""
                                ).toLocaleLowerCase("fa");


                            const table =
                                (
                                    card.dataset.reportTable
                                    ?? ""
                                ).toLocaleLowerCase("fa");


                            const matched =
                                !search ||
                                name.includes(search) ||
                                table.includes(search);


                            card.classList.toggle(
                                "hidden",
                                !matched);


                            if (matched)
                                visibleCount++;

                        });


                    emptySearch?.classList.toggle(
                        "hidden",
                        visibleCount !== 0);

                });

        }

    });


/*
 * =====================================================
 * Delete
 * =====================================================
 */

window.deleteReport =
    async function (
        reportId,
        reportName) {

        const confirmed =
            confirm(
                `آیا از حذف گزارش «${reportName}» مطمئن هستید؟`);


        if (!confirmed)
            return;


        try {

            const response =
                await fetch(
                    `${window.reportPage.urls.delete}?id=${reportId}`,
                    {
                        method: "DELETE"
                    });


            if (!response.ok)
                throw new Error(
                    "حذف گزارش ناموفق بود.");


            location.reload();

        }
        catch (error) {

            console.error(error);

            alert(
                "در حذف گزارش خطایی رخ داد.");

        }

    };