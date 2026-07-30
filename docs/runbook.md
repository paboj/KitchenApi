# Runbook - useful commands

## Starting the environment

```
docker-compose up -d
```

Starts `kitchen-api` (port 8080) and `kitchen-db` — PostgreSQL 16, container `kitchen-api-db`, database `KitchenDb`.

## Database backup

Dump in custom format (`pg_dump -Fc`) — compressed, allows selective restore, not human-readable.

```
docker exec -t kitchen-api-db pg_dump -U postgres -d KitchenDb -Fc -f /tmp/backup.dump
docker cp kitchen-api-db:/tmp/backup.dump ./backup.dump
```

`*.dump` files are gitignored — the backup stays local.

## Restoring from a backup

```
docker cp ./backup.dump kitchen-api-db:/tmp/backup.dump
docker exec -t kitchen-api-db pg_restore -U postgres -d KitchenDb --clean --if-exists /tmp/backup.dump
```

`--clean --if-exists` drops existing objects before restoring, to avoid conflicts (e.g. tables already existing from applied migrations).
