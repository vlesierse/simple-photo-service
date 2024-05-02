import { useState } from "react";
import { resendSignUpCode, confirmSignUp, autoSignIn } from "aws-amplify/auth";
import {
  Alert,
  Button,
  Container,
  Form,
  FormField,
  Header,
  SpaceBetween,
} from "@cloudscape-design/components";

import { Formik, FormikProps } from "formik";
import * as Yup from "yup";

import { Input } from "../../../components/forms";
import { AuthError, useAuth } from "../core";

type FormType = {
  email: string;
  code: string;
};

const FormValidation = Yup.object().shape({
  email: Yup.string().required("Required").email("Invalid email"),
  code: Yup.string()
    .required("Required")
    .matches(/^[0-9]+$/, "Must be only digits")
    .min(6, "Must be exactly 6 digits")
    .max(6, "Must be exactly 6 digits"),
});

export const ConfirmSignUp = () => {
  const { currentUser } = useAuth();
  const [remoteError, setRemoteError] = useState<string | undefined>();
  const [information, setInformation] = useState<string | undefined>();

  const handleSubmit = async (values: FormType) => {
    try {
      const { nextStep } = await confirmSignUp({
        username: values.email,
        confirmationCode: values.code,
      });
      if (nextStep.signUpStep === "COMPLETE_AUTO_SIGN_IN") {
        await autoSignIn();
      }
    } catch (error) {
      const err = error as AuthError;
      setRemoteError("Something went wrong! " + err.message);
    }
  };

  const handleResend = async (formik: FormikProps<FormType>) => {
    try {
      resendSignUpCode({ username: formik.values.email });
      setInformation("Confirmation code sent to your email.");
    } catch (error) {
      const err = error as AuthError;
      setRemoteError("Something went wrong! " + err.message);
    }
  };

  const initialValues: FormType = {
    email: currentUser?.getUsername() ?? "",
    code: "",
  };

  return (
    <Container header={<Header variant="h3">Confirm Sign Up</Header>}>
      <Formik
        validateOnBlur={true}
        validateOnChange={false}
        initialValues={initialValues}
        onSubmit={handleSubmit}
        validationSchema={FormValidation}
      >
        {(formik) => (
          <Form
            actions={
              <SpaceBetween direction="horizontal" size="xs">
                <Button
                  variant="primary"
                  onClick={() => formik.submitForm()}
                  disabled={formik.isSubmitting}
                >
                  Confirm
                </Button>
                <Button onClick={() => handleResend(formik)}>Resend</Button>
              </SpaceBetween>
            }
            errorText={remoteError}
          >
            <SpaceBetween size="l">
              <FormField label="Email address" errorText={formik.errors.email}>
                <Input name="email" placeholder="Email address" />
              </FormField>
              <FormField label="Code" errorText={formik.errors.code}>
                <Input name="code" placeholder="Confirmation code" />
              </FormField>
              {!information ?? (
                <Alert type="info" header="Resend">
                  {information}
                </Alert>
              )}
            </SpaceBetween>
          </Form>
        )}
      </Formik>
    </Container>
  );
};
