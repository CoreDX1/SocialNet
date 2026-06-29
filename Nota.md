## 1. Registro — `AuthController`

```bash
curl -X POST https://localhost:7xxx/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "christian",
    "email": "christian@test.com",
    "password": "Password123",
    "displayName": "Christian Dev"
  }'
```

**Respuesta esperada:**
```json
{
  "userId": "guid...",
  "username": "christian",
  "token": "eyJhbGciOi...",
  "expiration": "2025-..."
}
```

Copia el `token`, lo necesitas para todo lo demás.

---

## 2. Login — `AuthController`

```bash
curl -X POST https://localhost:7xxx/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "christian@test.com",
    "password": "Password123"
  }'
```

---

## 3. Variable de entorno para no repetir el token

```bash
TOKEN="eyJhbGciOi..."  # pega aquí tu token
```

---

## 4. Perfil de usuario — `UsersController`

```bash
# Ver perfil propio
curl https://localhost:7xxx/api/users/christian \
  -H "Authorization: Bearer $TOKEN"

# Actualizar perfil
curl -X PUT https://localhost:7xxx/api/users/me \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "displayName": "Christian Full Stack",
    "bio": "Desarrollador C# apasionado"
  }'

# Buscar usuarios
curl "https://localhost:7xxx/api/users/search?q=chris" \
  -H "Authorization: Bearer $TOKEN"
```

---

## 5. Posts — `PostsController`

```bash
# Crear post
curl -X POST https://localhost:7xxx/api/posts \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "content": "Mi primer post en SocialNet!"
  }'

# Ver feed (posts tuyos y de quienes sigues)
curl "https://localhost:7xxx/api/posts/feed?page=1&pageSize=10" \
  -H "Authorization: Bearer $TOKEN"

# Ver post específico (reemplaza POST_ID)
curl https://localhost:7xxx/api/posts/POST_ID \
  -H "Authorization: Bearer $TOKEN"

# Ver posts de un usuario
curl "https://localhost:7xxx/api/posts/user/christian?page=1&pageSize=10" \
  -H "Authorization: Bearer $TOKEN"

# Eliminar post
curl -X DELETE https://localhost:7xxx/api/posts/POST_ID \
  -H "Authorization: Bearer $TOKEN"
```

---

## 6. Likes — `LikesController`

```bash
# Dar/quitar like (toggle)
curl -X POST https://localhost:7xxx/api/posts/POST_ID/like \
  -H "Authorization: Bearer $TOKEN"
```

---

## 7. Comentarios — `CommentsController`

```bash
# Crear comentario
curl -X POST https://localhost:7xxx/api/posts/POST_ID/comments \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "content": "Gran post, me encantó!"
  }'

# Ver comentarios de un post
curl "https://localhost:7xxx/api/posts/POST_ID/comments?page=1&pageSize=20" \
  -H "Authorization: Bearer $TOKEN"

# Eliminar comentario
curl -X DELETE https://localhost:7xxx/api/posts/POST_ID/comments/COMMENT_ID \
  -H "Authorization: Bearer $TOKEN"
```

---

## 8. Seguir / Dejar de seguir — `FollowController`

Necesitas registrar otro usuario para probar esto:

```bash
# Registrar segundo usuario (AuthController)
curl -X POST https://localhost:7xxx/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "maria",
    "email": "maria@test.com",
    "password": "Password123",
    "displayName": "María López"
  }'

# Seguir a maria (toggle)
curl -X POST https://localhost:7xxx/api/users/maria/follow \
  -H "Authorization: Bearer $TOKEN"

# Ver seguidores de maria
curl "https://localhost:7xxx/api/users/maria/follow/followers?page=1&pageSize=20" \
  -H "Authorization: Bearer $TOKEN"

# Ver a quién sigue maria
curl "https://localhost:7xxx/api/users/maria/follow/following?page=1&pageSize=20" \
  -H "Authorization: Bearer $TOKEN"
```

---

## 9. Mensajes — `MessagesController`

```bash
# Enviar mensaje a maria (reemplaza MARIA_ID con su guid)
curl -X POST https://localhost:7xxx/api/messages \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "receiverId": "MARIA-GUID-AQUÍ",
    "content": "Hola María, ¿cómo estás?"
  }'

# Ver conversaciones
curl "https://localhost:7xxx/api/messages/conversations?page=1&pageSize=20" \
  -H "Authorization: Bearer $TOKEN"

# Ver mensajes con un usuario específico
curl "https://localhost:7xxx/api/messages/conversations/MARIA_ID?page=1&pageSize=50" \
  -H "Authorization: Bearer $TOKEN"

# Marcar como leídos
curl -X PUT https://localhost:7xxx/api/messages/conversations/MARIA_ID/read \
  -H "Authorization: Bearer $TOKEN"
```

---

## Resumen por controlador

| Controlador | Endpoints |
|---|---|
| **AuthController** | `POST /api/auth/register`, `POST /api/auth/login` |
| **UsersController** | `GET /api/users/{username}`, `PUT /api/users/me`, `POST /api/users/me/avatar`, `GET /api/users/search` |
| **PostsController** | `POST /api/posts`, `GET /api/posts/{id}`, `GET /api/posts/feed`, `GET /api/posts/user/{username}`, `DELETE /api/posts/{id}` |
| **LikesController** | `POST /api/posts/{id}/like` |
| **CommentsController** | `POST /api/posts/{id}/comments`, `GET /api/posts/{id}/comments`, `DELETE /api/posts/{id}/comments/{commentId}` |
| **FollowController** | `POST /api/users/{username}/follow`, `GET /api/users/{username}/follow/followers`, `GET /api/users/{username}/follow/following` |
| **MessagesController** | `POST /api/messages`, `GET /api/messages/conversations`, `GET /api/messages/conversations/{userId}`, `PUT /api/messages/conversations/{userId}/read` |

---

## Flujo de prueba

```
 1. AuthController       → register → obtienes token
 2. AuthController       → login → verificas credenciales
 3. UsersController      → PUT users/me → actualizas perfil
 4. PostsController      → POST posts → creas contenido
 5. PostsController      → GET posts/feed → ves tu timeline
 6. LikesController      → POST posts/{id}/like → das like
 7. CommentsController   → POST posts/{id}/comments → comenta
 8. AuthController       → register otro usuario
 9. FollowController     → POST users/{otro}/follow → sigues a alguien
10. MessagesController   → POST messages → envías mensaje privado
11. MessagesController   → GET conversations → ves tus chats
