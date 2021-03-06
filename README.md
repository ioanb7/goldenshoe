# **a** Golden Shoe experiment

## About
This is an experiment.

![Architecture diagram](https://user-images.githubusercontent.com/9282777/39178482-bb97a0cc-47a9-11e8-9eed-45ec71ae00e0.png)

## Ports used:
- backend-host (api gateway): 80:8080
- authenticator-service: 9000
- products-service: 9001
- orders-service: 9002
- tracking-service: 9003
- database: 3307:3306
- graphiql: 4000
- adminer: 8082:8080
- varnish: 8081:80
- zookeeper: 2181
- kafka: 9092

## Installation guide:
1. run backend/messages/build.cmd
2. run docker-compose up
3. navigate to 127.0.0.1:8082, connect to hostname "database" with username "root" and password "<empty>", select database products, and execute the SQL commands found in database-import/products.sql
4. edit `frontend/config.js`, replace the URL with your docker's virtual machine (if windows version < 10) or "127.0.0.1" otherwise.
5. cd into frontend/ and execute `npm i && npm run dev`
6. navigate to 127.0.0.1:3000

## Contact:

https://ioanb7.com