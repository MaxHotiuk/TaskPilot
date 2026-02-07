# Chat frontend integration

## SignalR

Hub endpoint: `/hubs/chat`

Client methods to call:
- `JoinUserGroup(userId)` to receive chat list updates (created/updated chats).
- `LeaveUserGroup(userId)` when disconnecting or switching user.
- `JoinChatGroup(chatId)` to receive new messages for a chat.
- `LeaveChatGroup(chatId)` when leaving a chat.
- `StartTyping(chatId, userId)` when the user begins typing.
- `StopTyping(chatId, userId)` when the user stops typing.

Server events:
- `ChatCreated` with `ChatDto` payload (new chat available for the user).
- `ChatUpdated` with `ChatDto` payload (chat metadata or last message changed).
- `ChatDeleted` with `{ chatId }` payload (remove chat from lists).
- `ReceiveChatMessage` with `ChatMessageDto` payload (new message in the chat).
- `ReceiveChatMessagePreview` with `ChatMessageDto` payload (use for toast notifications outside the chat view).
- `UserTyping` with `{ chatId, userId }` payload.
- `UserStoppedTyping` with `{ chatId, userId }` payload.

## REST endpoints

- `POST /api/chats`
  - Body: `CreateChatRequestDto`
  - Response: chat id (`Guid`)

- `GET /api/users/{userId}/chats?organizationId={orgId}`
  - Response: `IEnumerable<ChatDto>`

- `GET /api/chats/{chatId}/messages?userId={userId}&page={page}&pageSize={pageSize}`
  - Response: `IEnumerable<ChatMessageDto>`

- `POST /api/chats/{chatId}/messages`
  - Body: `SendChatMessageRequestDto` (uses `senderId` and `content`; `chatId` comes from the route)
  - Response: `ChatMessageDto`

- `PATCH /api/chats/{chatId}/read`
  - Body: `UpdateChatReadStatusRequestDto`
  - Response: `204 No Content`

- `PATCH /api/chats/{chatId}/name`
  - Body: `UpdateChatNameRequestDto`
  - Response: `204 No Content`

- `POST /api/chats/{chatId}/members`
  - Body: `UpdateChatMembersRequestDto`
  - Response: `204 No Content`

- `POST /api/chats/{chatId}/members/remove`
  - Body: `UpdateChatMembersRequestDto`
  - Response: `204 No Content`

- `DELETE /api/chats/{chatId}?userId={userId}`
  - Response: `204 No Content`

- `DELETE /api/chats/{chatId}/messages?userId={userId}`
  - Response: `204 No Content`

- `POST /api/chats/{chatId}/messages/{messageId}/attachments?userId={userId}`
  - Body: `multipart/form-data` with `file` field
  - Response: `AttachmentDto`
  - **Note:** `userId` query parameter is **required**

- `GET /api/attachments/{messageId}`
  - Response: `IEnumerable<AttachmentDto>` (returns `404` if no attachments exist)

## Payloads

`ChatMessageDto`
- `hasAttachments`: `bool`

`ChatMemberRole` enum: `Owner = 0`, `Member = 1`.
`ChatType` enum: `Private = 0`, `Group = 1`.

## Notes

- All chat participants must belong to the same organization.
- Private chats contain exactly two members; group chats require a name.
- Use `ChatMessageDto.hasAttachments` to show attachment icon.
- Avatar URLs: `/api/avatars/{userId}` (construct client-side).
