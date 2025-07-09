# Comments API Endpoints

This document describes the newly added comment endpoints for the TaskPilot API. These endpoints allow frontend developers to manage comments associated with tasks.

## Base URL
```
/api/comments
```

## Authentication
All endpoints require JWT authentication. Include the Bearer token in the Authorization header:
```
Authorization: Bearer <your-jwt-token>
```

## Endpoints Overview

| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| POST | `/api/comments` | Create a new comment | User Role |
| GET | `/api/comments/{id}` | Get a specific comment by ID | User Role |
| GET | `/api/tasks/{taskId}/comments` | Get all comments for a task | User Role |
| PUT | `/api/comments/{id}` | Update a comment | Comment Owner |
| DELETE | `/api/comments/{id}` | Delete a comment | Comment Owner |

## Detailed Endpoint Documentation

### 1. Create Comment
**POST** `/api/comments`

Creates a new comment for a task.

**Authorization:** Requires User Role

**Request Body:**
```json
{
  "taskId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "authorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "content": "This is a comment about the task"
}
```

**Response:**
- **201 Created** - Returns the created comment ID
```json
"3fa85f64-5717-4562-b3fc-2c963f66afa6"
```
- **400 Bad Request** - Invalid request data
- **401 Unauthorized** - Not authenticated
- **403 Forbidden** - Insufficient permissions
- **500 Internal Server Error** - Server error

---

### 2. Get Comment by ID
**GET** `/api/comments/{id}`

Retrieves a specific comment by its ID.

**Authorization:** Requires User Role

**Parameters:**
- `id` (path, required) - Comment ID (GUID)

**Response:**
- **200 OK** - Returns comment details
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "taskId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "authorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "content": "This is a comment about the task",
  "createdAt": "2025-07-08T10:30:00Z",
  "updatedAt": "2025-07-08T10:30:00Z"
}
```
- **401 Unauthorized** - Not authenticated
- **403 Forbidden** - Insufficient permissions
- **404 Not Found** - Comment not found
- **500 Internal Server Error** - Server error

---

### 3. Get Comments by Task ID
**GET** `/api/tasks/{taskId}/comments`

Retrieves all comments for a specific task.

**Authorization:** Requires User Role

**Parameters:**
- `taskId` (path, required) - Task ID (GUID)

**Response:**
- **200 OK** - Returns array of comments
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "taskId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "authorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "content": "First comment",
    "createdAt": "2025-07-08T10:30:00Z",
    "updatedAt": "2025-07-08T10:30:00Z"
  },
  {
    "id": "4fb85f64-5717-4562-b3fc-2c963f66afa7",
    "taskId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "authorId": "4fb85f64-5717-4562-b3fc-2c963f66afa8",
    "content": "Second comment",
    "createdAt": "2025-07-08T11:00:00Z",
    "updatedAt": "2025-07-08T11:00:00Z"
  }
]
```
- **401 Unauthorized** - Not authenticated
- **403 Forbidden** - Insufficient permissions
- **500 Internal Server Error** - Server error

---

### 4. Update Comment
**PUT** `/api/comments/{id}`

Updates an existing comment. Only the comment author or admin can update.

**Authorization:** Requires Comment Owner (author of the comment or admin)

**Parameters:**
- `id` (path, required) - Comment ID (GUID)

**Request Body:**
```json
{
  "content": "Updated comment content"
}
```

**Response:**
- **204 No Content** - Comment updated successfully
- **400 Bad Request** - Invalid request data
- **401 Unauthorized** - Not authenticated
- **403 Forbidden** - Not the comment owner or admin
- **404 Not Found** - Comment not found
- **500 Internal Server Error** - Server error

---

### 5. Delete Comment
**DELETE** `/api/comments/{id}`

Deletes an existing comment. Only the comment author or admin can delete.

**Authorization:** Requires Comment Owner (author of the comment or admin)

**Parameters:**
- `id` (path, required) - Comment ID (GUID)

**Response:**
- **204 No Content** - Comment deleted successfully
- **401 Unauthorized** - Not authenticated
- **403 Forbidden** - Not the comment owner or admin
- **404 Not Found** - Comment not found
- **500 Internal Server Error** - Server error

## Data Models

### CommentDto
```typescript
interface CommentDto {
  id: string;           // GUID
  taskId: string;       // GUID
  authorId: string;     // GUID
  content: string;
  createdAt: string;    // ISO 8601 datetime
  updatedAt: string;    // ISO 8601 datetime
}
```

### CreateCommentRequestDto
```typescript
interface CreateCommentRequestDto {
  taskId: string;       // GUID
  authorId: string;     // GUID
  content: string;
}
```

### UpdateCommentRequestDto
```typescript
interface UpdateCommentRequestDto {
  content: string;
}
```

## Authorization Rules

1. **User Role**: Basic authenticated users can:
   - Create comments
   - View any comment
   - View all comments for a task

2. **Comment Owner**: Users can edit and delete only their own comments
   - Comment ownership is determined by the `authorId` field
   - Admins can edit/delete any comment

3. **Admin Role**: Admins have full access to all comment operations

## Error Handling

All endpoints return standard HTTP status codes and follow the API's error response format:

```json
{
  "type": "https://example.com/probs/validation-error",
  "title": "Validation Error",
  "status": 400,
  "detail": "One or more validation errors occurred.",
  "errors": {
    "Content": ["The Content field is required."]
  }
}
```

## Usage Examples

### JavaScript/TypeScript Examples

#### Create a comment
```javascript
const createComment = async (taskId, authorId, content) => {
  const response = await fetch('/api/comments', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify({
      taskId,
      authorId,
      content
    })
  });
  
  if (response.ok) {
    const commentId = await response.json();
    return commentId;
  }
  throw new Error('Failed to create comment');
};
```

#### Get comments for a task
```javascript
const getTaskComments = async (taskId) => {
  const response = await fetch(`/api/tasks/${taskId}/comments`, {
    headers: {
      'Authorization': `Bearer ${token}`
    }
  });
  
  if (response.ok) {
    const comments = await response.json();
    return comments;
  }
  throw new Error('Failed to fetch comments');
};
```

#### Update a comment
```javascript
const updateComment = async (commentId, content) => {
  const response = await fetch(`/api/comments/${commentId}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify({ content })
  });
  
  if (!response.ok) {
    throw new Error('Failed to update comment');
  }
};
```

#### Delete a comment
```javascript
const deleteComment = async (commentId) => {
  const response = await fetch(`/api/comments/${commentId}`, {
    method: 'DELETE',
    headers: {
      'Authorization': `Bearer ${token}`
    }
  });
  
  if (!response.ok) {
    throw new Error('Failed to delete comment');
  }
};
```

## Notes for Frontend Developers

1. **GUIDs**: All IDs are in GUID format. Make sure to handle them as strings.

2. **Timestamps**: All datetime fields are in ISO 8601 format (UTC).

3. **Authorization**: The API will automatically determine if a user can edit/delete based on comment ownership. Handle 403 responses gracefully in your UI.

4. **Real-time Updates**: Consider implementing real-time updates for comments using SignalR or polling for better user experience.

5. **Validation**: Client-side validation should match server-side rules:
   - Content is required and should not be empty
   - TaskId and AuthorId must be valid GUIDs

6. **Error Handling**: Always handle potential network errors and display appropriate user feedback.
