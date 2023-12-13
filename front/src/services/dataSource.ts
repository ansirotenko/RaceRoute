import axios, { AxiosError, AxiosResponse } from 'axios';
import { GenerateArgs, Point, Race, Track } from './api';

export function timeout(ms: number) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

export async function fetchRaces() {
  return prettifyError(async () => {
    const result = await axios.get<{ races: Race[] }>(`/api/races`);
    return result.data.races;
  });
}

export async function fetchRaceInfo(raceId: number) {
  return prettifyError(async () => {
    const result = await axios.get<{ points: Point[]; tracks: Track[] }>(`/api/races/${raceId}`);
    return result.data;
  });
}

export function generateNewRace(args: GenerateArgs) {
  return prettifyError(async () => {
    const ret = await axios.put<Race>(`/api/races`, args);
    return ret.data;
  });
}

export function deleteRace(raceId: number) {
  return prettifyError(async () => {
    await axios.delete(`/api/races/${raceId}`);
  });
}

async function prettifyError<T>(body: () => Promise<T>) {
  try {
    return await body();
  } catch (e) {
    throw new Error(((e as AxiosError).response as AxiosResponse)?.data || "Unknown error");
  }
}
