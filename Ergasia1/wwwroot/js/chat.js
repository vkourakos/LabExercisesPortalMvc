const connection = new signalR.HubConnectionBuilder()
    .withUrl("/messageHub")
    .build();

async function startSignalR() {
    try {
        await connection.start();
        console.log("SignalR Connected");

        if (groupId !== "null") {
            await connection.invoke("JoinGroup", groupId);
        }
    } catch (err) {
        console.error("SignalR Connection Error:", err);
    }
}

connection.on("ReceiveMessage", (sender, message) => {
    let chatBox = document.getElementById("chatMessages");
    chatBox.innerHTML += `<p><b>${sender}:</b> ${message}</p>`;

    if (!document.hasFocus()) {
        showNotification(sender, message);
    }
});

function showNotification(sender, message) {
    if (Notification.permission === "granted") {
        const notification = new Notification(`${sender} sent a message`, {
            body: message,
        });

        notification.onclick = function () {
            window.focus();
        };
    }
}

function requestNotificationPermission() {
    if (Notification.permission !== "granted") {
        Notification.requestPermission().then(function (permission) {
            if (permission === "granted") {
                console.log("Notification permission granted.");
            }
        });
    }
}

async function sendMessage() {
    let content = document.getElementById("messageInput").value;
    if (!content.trim()) return;

    let data = new FormData();
    data.append("receiverId", receiverId);
    data.append("groupId", groupId);
    data.append("content", content);

    try {
        await fetch("/Message/SendMessage", {
            method: "POST",
            body: data
        });

        document.getElementById("messageInput").value = "";
    } catch (err) {
        console.error("Error sending message:", err);
    }
}
