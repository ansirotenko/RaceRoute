import { Point, Race, Track } from '../services/api';
import { useEffect, useState } from 'react';
import { fetchRaceInfo } from '../services/dataSource';
import Box from '@mui/material/Box';
import Alert from '@mui/material/Alert';
import CircularProgress from '@mui/material/CircularProgress';
import AlertTitle from '@mui/material/AlertTitle';
import RaceRoute from './RaceRoute';

interface SingleRaceProps {
  race: Race;
}

export default function SingleRace({ race }: SingleRaceProps) {
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);
  const [data, setData] = useState<{points: Point[], tracks: Track[]} | null>(null);

  useEffect(() => {
    async function loadData() {
      try {
        setError(null);
        setLoading(true);
        const response = await fetchRaceInfo(race.id);
        setData(response);
      } catch (e) {
        setError((e as Error).message || 'Error on loading race info');
      } finally {
        setLoading(false);
      }
    }

    if (race === null || race === undefined) {
      setError('Race is not set');
    } else {
      loadData();
    }
  }, [race]);

  if (error) {
    return (
      <Alert severity="error">
        <AlertTitle>Error</AlertTitle>
        {error}
      </Alert>
    );
  }

  if (loading) {
    return (
      <Box sx={{ my: '2rem', textAlign: 'center' }}>
        <CircularProgress color="inherit" />
      </Box>
    );
  }

  return <RaceRoute tracks={data!.tracks} points={data!.points}/>;
}
