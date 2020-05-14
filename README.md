# Grafana Helper - Function
Api functions for Grafana dashboard. 

## Enviroment
- `BlobConnectionString`: Connection string to blob storage
- `BlobContainerName`: Blob container name
- `IoTHubConnectionString`: IoT Hub connection string
- `DBConnection`: Connection string to database

## Setup
- Database must be initialized with `Log` table

## Functions

### AddLog:
Adds log entry to database.

Post:
```json
{
    DeviceId: "{deviceId}",
    Activity: "{activity_topic}",
    Log: "{log_message}",
}
```

### GetTwinInfo:
Gets twin properties for device.

Get: `?deviceid={deviceid}`

### PatchTwinInfo:
Updates twin data for device.

Post:
`?deviceid={deviceid}&etag={etag}&patch={twin_update}`

### SendDirectMethod:
Sends direct method to device.

Post:
```json
{
    DeviceId: "{deviceid}",
    Method: "{method_name}",
    Payload: "{method_payload}"
}
```

Returns:
```json
{
    Status: "{result_status}",
    ResultPayload: "{result_payload}"
}
```

### LastTimelapseImage:
Gets last blob in container and returns as jpg.

Get: