"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
var typingTimeout;
var typingIndicator = document.getElementById("typingIndicator");
var currentUserName = document.getElementById("currentUserName").value;
var msgList = document.getElementById("messagesList");
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    li.textContent = `${user} пишет: ${message}`;

    if (user == "admin") {
        li.style.color = "purple";
        var audio = document.getElementById("incomingMessageSound");
        audio.play();
    }
    else if (user !== currentUserName)
    {
        li.style.color = "blue";
    }
    else {
        li.style.color = "red";
        var audio = document.getElementById("incomingMessageSound");
        audio.play();
    }
    var messageList = msgList;
    msgList.insertBefore(li, messageList.firstChild);
    requestAnimationFrame(() => {
        li.classList.add("show");
    });
    requestAnimationFrame(() => {
        li.classList.add("blink");
    });
});

connection.on("UserTyping", function (user) {
    typingIndicator.textContent = `${user} пишет ...`;
    clearTimeout(typingTimeout);
    typingTimeout = setTimeout(() => {
        typingIndicator.textContent = '';
    }, 3000);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", currentUserName, message).catch(function (err) {
        return console.error(err.toString());
    });
    var audio = document.getElementById("buttonClickSound");
    audio.play();
    document.getElementById("messageInput").value = '';
    event.preventDefault();
});

document.getElementById("messageInput").addEventListener("input", function () {
    connection.invoke("Typing", currentUserName).catch(function (err) {
        return console.error(err.toString());
    });
});