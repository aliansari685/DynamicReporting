import { showNotification } from "./ui.js";


const connection =
    new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:7177/report-hub")
        .withAutomaticReconnect()
        .build();

export async function startSignalR() {

    try {

        await connection.start();

        console.log(
            "SignalR connected.");

    } catch (error) {

        //console.error(
        //    "SignalR connection failed.",
        //    error);

        setTimeout(
            startSignalR,
            5000);

    }

}


connection.on(
    "ReportReady",
    function (data) {

        showNotification(
            data.message ||
            "گزارش شما آماده دانلود است.",
            "success");
    });


export async function joinReportGroup(reportGuid) {

    try {

        await connection.invoke(
            "JoinGroup",
            reportGuid.toString());

    } catch (error) {

        console.error(
            "Could not join report group.",
            error);

    }

}