import { Controller, useForm, Control, RegisterOptions, FieldErrors } from 'react-hook-form';
import { GenerateArgs, Race } from '../services/api';
import { useState } from 'react';
import { generateNewRace } from '../services/dataSource';
import Box from '@mui/material/Box';
import TextField from '@mui/material/TextField';
import Alert from '@mui/material/Alert';
import Button from '@mui/material/Button';
import CircularProgress from '@mui/material/CircularProgress';
import Typography from '@mui/material/Typography';

interface NewRaceDialogProps {
  idTitle: string;
  idControls: string;
  onSuccess: (race: Race) => void;
}

export default function NewRaceDialog({ onSuccess, idTitle, idControls }: NewRaceDialogProps) {
  const [serverErrors, setServerErrors] = useState<string[] | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const {
    handleSubmit,
    formState: { errors, isValidating },
    control,
  } = useForm<GenerateArgs>({
    mode: 'onChange',
    defaultValues: {
      heightMean: 100,
      heightStddev: 10,
      distanceMean: 20,
      distanceStddev: 3,
      speedSmoothness: 0.65,
      surfaceSmoothness: 0.9,
      pointsNumber: 200,
    },
  });

  async function submit(data: GenerateArgs) {
    try {
      setServerErrors(null);
      setIsLoading(true);
      const ret = await generateNewRace(data);
      onSuccess(ret);
    } catch (e) {
      setServerErrors((e as Error).message.split('\n'));
    } finally {
      setIsLoading(false);
    }
  }

  const disabled = !errors || isLoading || isValidating;

  return (
    <>
      <Typography id={idTitle} variant="h6" component="h2">
        Generate a new race
      </Typography>
      <Box id={idControls} sx={{ mt: 2 }}>
        <form onSubmit={handleSubmit(submit)} autoComplete="off" noValidate>
          <Box sx={{ disabled: 'flex', flexDirection: 'column' }}>
            <GenerateArgsField
              name="heightMean"
              control={control}
              label="Height mean"
              errors={errors}
              validation={{
                min: {
                  value: 0.001,
                  message: 'Height mean should be more than 0',
                },
              }}
            />
            <GenerateArgsField
              name="heightStddev"
              control={control}
              label="Height standard deviation"
              errors={errors}
              validation={{
                min: {
                  value: 0.001,
                  message: 'Height standard deviation should be more than 0',
                },
              }}
            />
            <GenerateArgsField
              name="distanceMean"
              control={control}
              label="Distance mean"
              errors={errors}
              validation={{
                min: {
                  value: 0.001,
                  message: 'Distance mean should be more than 0',
                },
              }}
            />
            <GenerateArgsField
              name="distanceStddev"
              control={control}
              label="Distance standard deviation"
              errors={errors}
              validation={{
                min: {
                  value: 0.001,
                  message: 'Distance standard deviation should be more than 0',
                },
              }}
            />
            <GenerateArgsField
              name="surfaceSmoothness"
              control={control}
              label="Surface smoothness"
              errors={errors}
              validation={{
                min: {
                  value: 0.001,
                  message: 'Surface smoothness should be more than 0',
                },
                max: {
                  value: 1,
                  message: 'Surface smoothness should be more less than 1',
                },
              }}
            />
            <GenerateArgsField
              name="speedSmoothness"
              control={control}
              label="Speed smoothness"
              errors={errors}
              validation={{
                min: {
                  value: 0.001,
                  message: 'Speed smoothness should be more than 0',
                },
                max: {
                  value: 1,
                  message: 'Speed smoothness should be more less than 1',
                },
              }}
            />
            <GenerateArgsField
              name="pointsNumber"
              control={control}
              label="Points number"
              errors={errors}
              validation={{
                min: {
                  value: 10,
                  message: 'Points number should be more than 10',
                },
              }}
            />

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
          </Box>

          <Box sx={{ mt: '1rem', display: 'flex', flexDirection: 'row', justifyContent: 'flex-end' }}>
            <Button type="submit" color="primary" disabled={disabled} variant="contained">
              New Race
            </Button>
          </Box>
        </form>
      </Box>
    </>
  );
}

interface GenerateArgsFieldProps {
  name: keyof GenerateArgs;
  label: string;
  control: Control<GenerateArgs>;
  validation: Omit<
    RegisterOptions<GenerateArgs, keyof GenerateArgs>,
    'valueAsNumber' | 'valueAsDate' | 'setValueAs' | 'disabled'
  >;
  errors: FieldErrors<GenerateArgs>;
}

function GenerateArgsField({ control, name, label, validation, errors }: GenerateArgsFieldProps) {
  return (
    <Controller
      name={name}
      control={control}
      rules={{
        ...validation,
        required: `${label} is required`,
      }}
      render={({ field, fieldState }) => (
        <TextField
          sx={{ width: '100%', my: '0.5rem' }}
          value={field.value}
          onChange={(e) => {
            field.onChange(e);
          }}
          error={fieldState.isDirty && !!errors[name]}
          id={field.name}
          label={label}
          helperText={fieldState.isDirty && errors[name] != null && errors[name]?.message}
        />
      )}
    />
  );
}
