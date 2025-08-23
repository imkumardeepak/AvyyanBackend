# Simplified Chat and Notification System Usage Guide

This document explains how to use the new simplified chat and notification controllers that operate without database persistence, using only in-memory storage and real-time communication.

## Overview

The simplified system includes:
- **SimpleNotificationController**: Sends real-time notifications without database storage
- **SimpleChatController**: Manages in-memory chat rooms for group and direct messaging
- **SimpleNotificationHub**: SignalR hub for real-time notifications
- **SimpleChatHub**: SignalR hub for real-time messaging

## Notification System

### API Endpoints

#### Send Notification to User
```http
POST /api/simple-notifications/send-to-user
Content-Type: application/json
Authorization: Bearer {token}

{
  "userId": 123,
  "title": "New Message",
  "message": "You have received a new message",
  "type": "Info",
  "category": "Chat",
  "actionUrl": "/chat/room/abc123",
  "actionText": "View Message"
}
```

#### Send Notification to Multiple Users
```http
POST /api/simple-notifications/send-to-users
Content-Type: application/json
Authorization: Bearer {token}

{
  "userIds": [123, 456, 789],
  "title": "System Update",
  "message": "The system will be updated tonight",
  "type": "Warning",
  "category": "System"
}
```

#### Send System Notification (Admin only)
```http
POST /api/simple-notifications/send-system
Content-Type: application/json
Authorization: Bearer {token}

{
  "title": "Maintenance Notice",
  "message": "System maintenance scheduled for tonight",
  "type": "System",
  "actionUrl": "/maintenance"
}
```

#### Send Notification to Roles (Admin/Manager only)
```http
POST /api/simple-notifications/send-to-roles
Content-Type: application/json
Authorization: Bearer {token}

{
  "roles": ["Admin", "Manager"],
  "title": "Admin Alert",
  "message": "Important admin notification",
  "type": "Warning"
}
```

### SignalR Connection (JavaScript)
```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/simple-notifications", {
        accessTokenFactory: () => authToken
    })
    .build();

// Listen for notifications
connection.on("ReceiveNotification", (notification) => {
    console.log("Received notification:", notification);
    // Display notification to user
    showNotification(notification.title, notification.message, notification.type);
});

// Start connection
connection.start();
```

## Chat System

### API Endpoints

#### Create Chat Room
```http
POST /api/simple-chat/rooms
Content-Type: application/json
Authorization: Bearer {token}

{
  "name": "Project Discussion",
  "type": "Group",
  "memberIds": [123, 456, 789]
}
```

#### Get User's Chat Rooms
```http
GET /api/simple-chat/rooms
Authorization: Bearer {token}
```

#### Create/Get Direct Chat
```http
POST /api/simple-chat/direct/456
Authorization: Bearer {token}
```

#### Join Chat Room
```http
POST /api/simple-chat/rooms/{roomId}/join
Authorization: Bearer {token}
```

#### Add Members to Room
```http
POST /api/simple-chat/rooms/{roomId}/members
Content-Type: application/json
Authorization: Bearer {token}

{
  "roomId": "abc123",
  "userIds": [999, 888]
}
```

### SignalR Connection (JavaScript)
```javascript
const chatConnection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/simple-chat", {
        accessTokenFactory: () => authToken
    })
    .build();

// Listen for messages
chatConnection.on("ReceiveMessage", (message) => {
    console.log("Received message:", message);
    displayMessage(message);
});

// Listen for typing indicators
chatConnection.on("UserTyping", (typingInfo) => {
    showTypingIndicator(typingInfo.userName, typingInfo.roomId);
});

chatConnection.on("UserStoppedTyping", (typingInfo) => {
    hideTypingIndicator(typingInfo.userName, typingInfo.roomId);
});

// Send message
function sendMessage(roomId, content) {
    chatConnection.invoke("SendMessage", {
        roomId: roomId,
        content: content,
        messageType: "Text"
    });
}

// Join room
function joinRoom(roomId) {
    chatConnection.invoke("JoinRoom", roomId);
}

// Start typing
function startTyping(roomId) {
    chatConnection.invoke("StartTyping", roomId);
}

// Stop typing
function stopTyping(roomId) {
    chatConnection.invoke("StopTyping", roomId);
}

// Start connection
chatConnection.start();
```

## Key Features

### Notifications
- ✅ Real-time delivery via SignalR
- ✅ Send to individual users
- ✅ Send to multiple users (bulk)
- ✅ Send system-wide notifications
- ✅ Send to users with specific roles
- ✅ No database storage (in-memory only)
- ✅ Connection tracking and statistics

### Chat
- ✅ Group chat rooms
- ✅ Direct (person-to-person) chat
- ✅ Real-time messaging via SignalR
- ✅ Typing indicators
- ✅ Online user tracking
- ✅ In-memory room management
- ✅ No message history persistence

## Important Notes

1. **No Persistence**: Messages and notifications are not stored in the database. They exist only in memory and are lost when the server restarts.

2. **Connection-Based**: Users must be connected via SignalR to receive real-time notifications and messages.

3. **Scalability**: This implementation uses in-memory storage, so it's suitable for single-server deployments. For multi-server scenarios, consider using Redis or similar distributed cache.

4. **User Management**: The system assumes users exist in the database for authentication, but chat rooms and messages are managed in-memory.

5. **Room IDs**: Chat rooms use string-based IDs (GUIDs for group chats, formatted strings for direct chats).

## Migration from Database-Backed System

The original database-backed controllers and hubs are still available:
- `/api/chat` (original ChatController)
- `/api/notifications` (original NotificationsController)
- `/hubs/chat` (original ChatHub)
- `/hubs/notifications` (original NotificationHub)

You can gradually migrate to the simplified system or use both in parallel.
