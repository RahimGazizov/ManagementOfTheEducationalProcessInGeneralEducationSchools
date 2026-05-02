document.getElementById("on").addEventListener("click", async () => {
    await fetch("/AdminPersonalAccount/MaintenanceOn", { method: "POST" });
    console.log("Maintenance ON");
});

document.getElementById("off").addEventListener("click", async () => {
    await fetch("/AdminPersonalAccount/MaintenanceOff", { method: "POST" });
    console.log("Maintenance OFF");
});