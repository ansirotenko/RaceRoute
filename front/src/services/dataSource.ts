import axios from 'axios';
import { GenerateArgs, Point, Race, Track } from './api';

// export function timeout(ms: number) {
//   return new Promise((resolve) => setTimeout(resolve, ms));
// }

export async function fetchRaces() {
  const result = await axios.get<{ races: Race[] }>(`/api/races`);
  return result.data.races;
}

export async function fetchRaceInfo(raceId: number) {
  const result = await axios.get<{ points: Point[]; tracks: Track[] }>(`/api/races?raceId=${raceId}`);
  return result.data;
}

export async function generateNewRace(args: GenerateArgs) {
  await axios.put(`/api/races`, args);
}

export async function deleteRace(raceId: number) {
  await axios.delete(`/api/races?raceId=${raceId}`);
}
