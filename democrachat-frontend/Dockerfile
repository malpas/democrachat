FROM node AS build
WORKDIR /build
RUN npm install -D parcel@next
COPY . .
RUN npm install
RUN npm run-script build 

FROM nginx:stable
WORKDIR /app
COPY --from=build /build/dist/prod .
COPY nginx.conf /etc/nginx/conf.d/default.conf