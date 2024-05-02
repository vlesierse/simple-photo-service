import { useState } from "react";
import { signIn } from "aws-amplify/auth";
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
import { AuthError } from "../core";
import { useNavigate } from "react-router-dom";

const FormValidation = Yup.object().shape({
  email: Yup.string().email("Invalid email").required("Required"),
  password: Yup.string().required("Required"),
});

export const SignIn = () => {
  const navigate = useNavigate();
  const [remoteError, setRemoteError] = useState<string | undefined>();

  return (
    <Container header={<Header variant="h3">Sign In</Header>}>
      <Formik
        initialValues={{ email: "", password: "" }}
        onSubmit={async (values) => {
          try {
            await signIn({ username: values.email, password: values.password });
          } catch (error) {
            const err = error as AuthError;
            if (err.name === "UserNotConfirmedException") {
              navigate("/auth/signup/confirm", {
                state: { email: values.email },
              });
            }
            setRemoteError("Something went wrong! " + err.message);
          }
        }}
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
                  Sign In
                </Button>
                <Button onClick={() => navigate("/auth/signup")}>
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
            </SpaceBetween>
          </Form>
        )}
      </Formik>
    </Container>
  );
};
