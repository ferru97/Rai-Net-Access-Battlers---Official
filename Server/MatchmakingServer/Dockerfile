FROM node:latest

WORKDIR /app

COPY . .

RUN rm -rf node_modules/

RUN npm install

EXPOSE 8000

ENTRYPOINT ["npm", "start"]