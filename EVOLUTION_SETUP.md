# Evolution API Setup Guide

This guide explains how to set up Evolution API instances and fix common errors like the 404 when sending messages.

## Understanding Instance Name

**IMPORTANT:** The `instanceName` is NOT the instance ID you see in the Evolution Manager UI.

- ✅ **Instance Name**: A custom name YOU choose when creating the instance (e.g., "debitum-instance")
- ❌ **Instance ID**: The UUID shown in Evolution Manager (e.g., "550e8400-e29b-41d4-a716-446655440000")

## Step-by-Step Setup

### 1. Start Evolution API

```bash
docker-compose up -d evolution-api
```

Wait for the service to be fully ready (check logs):
```bash
docker logs -f debitum-evolution-api
```

### 2. Access Evolution Manager

Open your browser:
- URL: http://localhost:8080/manager
- API Key: `change-this-super-secret-key`

### 3. Create an Instance

1. Click on **"Create Instance"**
2. Fill in the form:
   - **Instance Name**: `debitum-instance` (this is what you use in the API!)
   - **QR Code**: Will be generated automatically
   - **Webhook URL** (optional): Leave empty for now

3. Click **"Create"**

### 4. Connect WhatsApp

1. Open WhatsApp on your phone
2. Go to **Settings** → **Linked Devices** → **Link a Device**
3. Scan the QR code shown in Evolution Manager
4. Wait for status to change to **"open"** or **"connected"**

### 5. Test the Connection

Check if the instance is connected:

```http
GET http://localhost:8080/instance/connectionState/debitum-instance
Headers:
  apikey: change-this-super-secret-key
```

**Expected Response:**
```json
{
  "instance": {
    "instanceName": "debitum-instance",
    "status": "open"
  },
  "state": "open"
}
```

### 6. Send a Test Message

```http
POST http://localhost:8080/message/sendText/debitum-instance
Headers:
  apikey: change-this-super-secret-key
  Content-Type: application/json

Body:
{
  "number": "5511999999999",
  "text": "Hello from Debitum!"
}
```

## Common Errors

### 404 Error when sending message

**Cause:** Instance doesn't exist or wrong instance name

**Solution:**
1. Verify instance exists in Evolution Manager
2. Check you're using the **instance name**, not the instance ID
3. Ensure instance status is "open" (WhatsApp connected)

### Instance not connecting

**Cause:** QR code expired or WhatsApp disconnected

**Solution:**
1. Delete the instance in Evolution Manager
2. Create a new instance
3. Scan the QR code immediately
4. Don't close WhatsApp while scanning

### API Key error

**Cause:** Wrong API key in headers

**Solution:**
- Verify API key matches `docker-compose.yml`:
  ```yaml
  AUTHENTICATION_API_KEY: change-this-super-secret-key
  ```

## Using in Debitum API

Once your instance is configured:

**Send Payment Reminder:**
```http
POST http://localhost:5000/api/debtors/send-reminder
Content-Type: application/json

{
  "debtorId": "550e8400-e29b-41d4-a716-446655440000"
}
```

The Debitum API will:
1. Get debtor information from database
2. Build payment reminder message
3. Send via WhatsApp using Evolution API
4. Record that reminder was sent

## Instance Management

### List all instances

```bash
GET http://localhost:8080/instance/fetchInstances
Headers:
  apikey: change-this-super-secret-key
```

### Delete an instance

```bash
DELETE http://localhost:8080/instance/delete/debitum-instance
Headers:
  apikey: change-this-super-secret-key
```

### Restart an instance

```bash
PUT http://localhost:8080/instance/restart/debitum-instance
Headers:
  apikey: change-this-super-secret-key
```

## Production Recommendations

1. **Change the API Key**: Update `AUTHENTICATION_API_KEY` in docker-compose.yml
2. **Use Environment Variables**: Store sensitive data in `.env` file
3. **Enable Webhooks**: Configure webhook URL to receive message events
4. **Database Backup**: Evolution stores data in PostgreSQL - configure backups
5. **Multiple Instances**: Create one instance per customer for multi-tenant support

## Troubleshooting

**Check Evolution API logs:**
```bash
docker logs -f debitum-evolution-api
```

**Check if Evolution can reach PostgreSQL:**
```bash
docker exec -it debitum-evolution-api ping postgres
```

**Restart Evolution API:**
```bash
docker-compose restart evolution-api
```
