import http from 'k6/http';
import { sleep } from 'k6';

export let options = {
    vus: 1,
    duration: '200s'
}

export default function (data) {
    http.get("https://sdw-wsrest.ecb.europa.eu/service/data/EXR/D.PLN.EUR.SP00.A?startPeriod=2020-01-01&endPeriod=2020-01-07");
    sleep(15);
}