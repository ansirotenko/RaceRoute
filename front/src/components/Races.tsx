import { SyntheticEvent, useEffect, useState } from 'react';
import { fetchRaces } from '../services/dataSource';
import { Race } from '../services/api';
import Tabs from '@mui/material/Tabs';
import Tab from '@mui/material/Tab';
import Typography from '@mui/material/Typography';
import Box from '@mui/material/Box';
import CircularProgress from '@mui/material/CircularProgress';
import Alert from '@mui/material/Alert/Alert';
import AlertTitle from '@mui/material/AlertTitle';
import Button from '@mui/material/Button';
import AppBar from '@mui/material/AppBar';
import Container from '@mui/system/Container';
import Toolbar from '@mui/material/Toolbar';
import Modal from '@mui/material/Modal';
import logo from '../assets/race.svg';

const modalStyle = {
  position: 'absolute',
  top: '50%',
  left: '50%',
  transform: 'translate(-50%, -50%)',
  width: 400,
  bgcolor: 'background.paper',
  border: '2px solid #000',
  boxShadow: 24,
  p: 4,
};

export default function VerticalTabs() {
  const [open, setOpen] = useState(false);
  const [value, setValue] = useState(0);
  const [loading, setLoading] = useState(true);
  const [hasError, setHasError] = useState(false);
  const [races, setRaces] = useState<Race[]>([]);

  // eslint-disable-next-line react-hooks/exhaustive-deps
  useEffect(() => {
    async function loadData() {
      try {
        const response = await fetchRaces();
        setRaces(response);
      } catch (e) {
        setHasError(true);
      } finally {
        setLoading(false);
      }
    }
    loadData();
  }, []);

  function content() {
    if (loading) {
      return (
        <Container maxWidth="lg" sx={{my: '2rem', textAlign: 'center'}}>
          <CircularProgress color="inherit" />
        </Container>
      );
    }

    if (hasError) {
      return (
        <Alert severity="error">
          <AlertTitle>Error</AlertTitle>
          Error on loading races
        </Alert>
      );
    }

    if (!races.length) {
      return <Alert severity="info">No races yet. Please please crate one.</Alert>;
    }

    const handleChange = (_: SyntheticEvent, newValue: number) => {
      setValue(newValue);
    };

    return (
      <Box sx={{ flexGrow: 1, bgcolor: 'background.paper', display: 'flex', height: '100vh' }}>
        <Tabs
          orientation="vertical"
          variant="scrollable"
          value={value}
          onChange={handleChange}
          aria-label="Races"
          sx={{ borderRight: 1, borderColor: 'divider' }}
        >
          {races.map((r, i) => (
            <Tab key={r.id} label={r.name} id={`vertical-tab-${i}`} aria-controls={`vertical-tabpanel-${i}`} />
          ))}
        </Tabs>
        <Box sx={{ p: 3 }}>
          <Typography component="h1">{races[value].name}</Typography>
        </Box>
      </Box>
    );
  }

  return (
    <>
      <AppBar position="static" sx={{ bgcolor: 'background.paper', mb: '1rem' }}>
        <Box sx={{mx: "1.5rem"}}>
          <Toolbar disableGutters sx={{ display: 'flex', flexGrow: 1 }}>
            <img src={logo} height="40px" />
            <div style={{ flexGrow: 1 }} />
            <Button variant="outlined" onClick={() => setOpen(true)}>
              New race
            </Button>
          </Toolbar>
        </Box>
      </AppBar>
      {content()}
      <Modal
        open={open}
        onClose={() => setOpen(false)}
        aria-labelledby="modal-modal-title"
        aria-controls="modal-modal-controls"
      >
        <Box sx={modalStyle}>
          <Typography id="modal-modal-title" variant="h6" component="h2">
            Generate a new race
          </Typography>
          <Typography id="modal-modal-controls" sx={{ mt: 2 }}>
            asdad
          </Typography>
        </Box>
      </Modal>
    </>
  );
}
