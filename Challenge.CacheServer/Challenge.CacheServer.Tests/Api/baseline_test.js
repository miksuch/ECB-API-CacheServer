import http from 'k6/http';
import { sleep } from 'k6';

export let options = {
    vus: 1,
    duration: "100s",
}

const apiUrl = "https://localhost:5001/";
const exchangeRatesResourcePath = "currencies/exchange-rates/";
const apiKeyResourcePath = "api/api-key"

function encodeData(data) {
    return Object.keys(data).map(function (key) {
        return [key, data[key]].map(encodeURIComponent).join("=");
    }).join("&");
}

export function setup() {
    let apiKey = http.get(apiUrl + apiKeyResourcePath).body;
    let currency0 = "currencies[PLN]=EUR";
    let currency1 = "currencies[EUR]=PLN";
    let queryParametersObject = {
        startPeriod: "2021-10-20",
        endPeriod: "2021-10-22",
        apiKey: apiKey
    }
    let queryParameters = encodeData(queryParametersObject);
    let query = apiUrl + exchangeRatesResourcePath + "?" + currency0 + '&' + queryParameters;

    console.log("using api key:" + apiKey);
    console.log("using api endpoint:" + query);

    return {
        apiKey: apiKey,
        query: query
    };
}

export default function (data) {
    http.get(data.query);
    sleep(15);
}
