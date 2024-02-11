import http from "k6/http";
import { sleep } from "k6";

export default function () {
  let username = "tadmin";
  let password = "$Sanjana1";

  let url = "https://localhost:5001/api/auth/login";
  let payload = JSON.stringify({ username: username, password: password });

  let params = {
    headers: {
      "Content-Type": "application/json",
    },
  };

  let response = http.post(url, payload, params);
  console.log("Response status code:", response.status);

  sleep(1);
}
