import React from "react";
import {
  Input as CSInput,
  InputProps as CSInputProps,
} from "@cloudscape-design/components";
import { FormikFieldProps } from "./FormikFieldProps";
import { Field, FieldProps } from "formik";
import { Optional } from "../types";

export type InputProps = FormikFieldProps & CSInputProps;

interface InputType
  extends React.ForwardRefExoticComponent<
    FormikFieldProps &
      Optional<CSInputProps, "value"> &
      React.RefAttributes<CSInputProps.Ref>
  > {}

const Input = React.forwardRef(
  (
    {
      name,
      validate,
      fast,
      onChange: $onChange,
      onBlur: $onBlur,
      ...restProps
    }: InputProps,
    ref: React.Ref<CSInputProps.Ref>
  ) => (
    <Field name={name} validate={validate} fast={fast}>
      {({ field, form }: FieldProps) => (
        <CSInput
          ref={ref}
          name={name}
          onChange={(event) => {
            form.setFieldValue(name, event.detail.value);
            $onChange && $onChange(event);
          }}
          onBlur={(event) => {
            form.validateField(name);
            $onBlur && $onBlur(event);
          }}
          {...restProps}
          value={field.value}
        />
      )}
    </Field>
  )
);

const TypedInput = Input as unknown as InputType;

export { TypedInput as Input };
export default TypedInput;
