FROM node AS build
WORKDIR /build
RUN yarn add -D parcel@next
COPY . .
RUN yarn install
RUN yarn run build 

FROM nginx:stable-alpine
WORKDIR /app
COPY --from=build /build/dist/prod .
COPY nginx.conf /etc/nginx/conf.d/default.conf
