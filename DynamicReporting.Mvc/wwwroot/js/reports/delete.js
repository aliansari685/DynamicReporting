import { urls } from "./state.js";

import {
    showNotification,
    showConfirm
} from "./ui.js";

export async function deleteReport(id) {

    const confirmed =
        await showConfirm(
            "آیا از حذف این گزارش مطمئن هستید؟",
            "حذف گزارش");

    if (!confirmed)
        return;

    const response =
        await fetch(
            `${urls.delete}?id=${id}`,
            {
                method: "DELETE"
            });


    if (!response.ok) {

        showNotification(
            "حذف گزارش انجام نشد.",
            "error");

        return;
    }


    showNotification(
        "گزارش با موفقیت حذف شد.",
        "success");


    setTimeout(
        () => location.href =
            urls.index,
        500);

}

