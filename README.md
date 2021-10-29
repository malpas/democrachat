# Introduction

An experimental chat site/platform written in ASP.NET and React, testing organic community moderation through a two-tier "voting" currency. Silver lets users mute each other and "bid" for new topics. Gold determines how much silver a user generates an hour. Determining how to actually hand out gold is left as an exercise to the admin.

The platform is designed to be as transparent as possible through a public transparency log that lists all user moderation actions (i.e mutes).

The platform was run publically on the internet for a few months and demonstrated the whole idea is (probably) a mistake. Docker images have been set up for convenience of use and deployment.

# Features
* Text chat with user-selectable topics
* Moderation through experimental two-tier "voting" currency
* Video chat with [PeerJS](https://github.com/peers/peerjs)
* User inventory -- users can gift each other random items

# Usage
## Requirements
* Docker & docker-compose
* Liquibase

## Running
1. (Optional) For better security, change the database password (`CHANGETHIS` in main docker-compose.yml).
2. Run `docker-compose build` then `docker-compose up` to start the application. You may need to create a Docker volume for the database. 
2. After the initial startup, the database will not be setup. Switch to the `Democrachat/Migrations` folder. [Set up Liquibase for Postgres](https://docs.liquibase.com/workflows/database-setup-tutorials/postgresql.html). Create a `liquibase.properties` file. Finally, run `liquibase update --changeLogFile=liquibase_changelog.xml`.

Now go to `http://localhost:8080` This is enough to test the chatting features. To test the experimental moderation features:

1. Finalise a user with any name (e.g. "admin")
2. Run `docker exec -it democrachat_postgres_1 psql -U postgres -d democrachat -c "UPDATE account SET gold = 999, silver = 999 WHERE username = 'admin'"`

### Example liquibase.properties
```
url: jdbc:postgresql://localhost:5432/democrachat?user=postgres&password=CHANGETHIS
```

## Automated Tests

There are automated integration and unit tests. With the postgres image up, run `CUSTOMCONNSTR_Default="Host=localhost;Database=democrachat;Username=postgres;Password=CHANGETHIS;" dotnet test`

Replace `CHANGETHIS` with whatever database password you selected.
