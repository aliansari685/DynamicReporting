import { urls } from "./state.js";
import { showNotification } from "./ui.js";

export async function deleteReport(id) {

    if (!confirm(
        "آیا از حذف این گزارش مطمئن هستید؟"))
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

