import { useState } from 'react';
import Box from '@mui/material/Box';
import Alert from '@mui/material/Alert';
import CircularProgress from '@mui/material/CircularProgress';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';
import { deleteRace } from '../services/dataSource';
import { Race } from '../services/api';

interface NewRaceDialogProps {
  idControls: string;
  idTitle: string;
  race: Race;
  onDelete: () => void;
}

export default function DeleteRaceDialog({ onDelete, idTitle, race, idControls }: NewRaceDialogProps) {
  const [serverErrors, setServerErrors] = useState<string[] | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  return (
    <>
      <Typography sx={{textAlign: 'center'}} id={idTitle} variant="h6" component="h2">
        Do you really want to delete {race.name}?
      </Typography>
      <Box id={idControls} sx={{ mt: 2 }}>
        {serverErrors &&
          serverErrors.map((e, i) => (
            <Alert sx={{ my: '0.5rem' }} key={`key${i}`} variant="outlined" severity="error">
              {e}
            </Alert>
          ))}
        {isLoading && (
          <Box sx={{ textAlign: 'center', my: '0.5rem' }}>
            <CircularProgress color="inherit" />
          </Box>
        )}
        <Box sx={{ mt: '1rem', display: 'flex', flexDirection: 'row', justifyContent: 'flex-end' }}>
          <Button
            variant="outlined"
            color="error"
            onClick={async () => {
              try {
                setServerErrors(null);
                setIsLoading(true);
                await deleteRace(race.id);
                onDelete();
              } catch (e) {
                setServerErrors((e as Error).message.split('\n'));
              } finally {
                setIsLoading(false);
              }
            }}
            disabled={isLoading}
          >
            Delete
          </Button>
        </Box>
      </Box>
    </>
  );
}
