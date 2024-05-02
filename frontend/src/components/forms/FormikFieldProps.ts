export interface FormikFieldProps {
  name: string;
  validate?: (value: unknown) => undefined | string | Promise<unknown>;
  fast?: boolean;
}
