import { useState } from "react";
import { AuthError, signUp } from "aws-amplify/auth";
import {
  Button,
  Container,
  Form,
  FormField,
  Header,
  SpaceBetween,
} from "@cloudscape-design/components";

import { Formik } from "formik";
import * as Yup from "yup";

import { Input } from "../../../components/forms";
import { useNavigate } from "react-router-dom";

type FormType = {
  email: string;
  password: string;
  passwordConfirmation: string;
};

const FormValidation = Yup.object().shape({
  email: Yup.string().email("Invalid email").required("Required"),
  password: Yup.string().required("Password is required"),
  passwordConfirmation: Yup.string().oneOf(
    [Yup.ref("password")],
    "Passwords must match"
  ),
});

export const SignUp = () => {
  const navigate = useNavigate();
  const [remoteError, setRemoteError] = useState<string | undefined>();

  const handleSubmit = async ({ email, password }: FormType) => {
    try {
      const result = await signUp({
        username: email,
        password,
        options: {
          userAttributes: { email },
          autoSignIn: { enabled: true },
        },
      });
      if (!result.isSignUpComplete) navigate("/auth/signup/confirm");
    } catch (error) {
      const err = error as AuthError;
      setRemoteError("Something went wrong! " + err.message);
    }
  };

  return (
    <Container header={<Header variant="h3">Sign Up</Header>}>
      <Formik
        initialValues={{ email: "", password: "", passwordConfirmation: "" }}
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
                  Sign Up
                </Button>
              </SpaceBetween>
            }
            errorText={remoteError}
          >
            <SpaceBetween size="l">
              <FormField label="Email address" errorText={formik.errors.email}>
                <Input name="email" placeholder="Email address" />
              </FormField>
              <FormField label="Password" errorText={formik.errors.password}>
                <Input name="password" type="password" placeholder="Password" />
              </FormField>
              <FormField
                label="Confirm password"
                errorText={formik.errors.passwordConfirmation}
              >
                <Input
                  name="passwordConfirmation"
                  type="password"
                  placeholder="Password"
                />
              </FormField>
            </SpaceBetween>
          </Form>
        )}
      </Formik>
    </Container>
  );
};
