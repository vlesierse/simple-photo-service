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
import { Input } from "../../components/forms";
import { useNavigate } from "react-router-dom";
import { post } from "aws-amplify/api";

type FormType = {
  title: string;
};

const FormValidation = Yup.object().shape({
  title: Yup.string().required("Required"),
});

export const CreateAlbum = () => {
  const navigate = useNavigate();
  const handleSubmit = async ({ title }: FormType) => {
    await post({
      apiName: "AppApi",
      path: "/albums",
      options: { body: { title } },
    });
    navigate(-1);
  };

  const initialValues: FormType = {
    title: "",
  };

  return (
    <Container header={<Header variant="h3">Create album</Header>}>
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
                  Create
                </Button>
                <Button
                  onClick={() => navigate(-1)}
                  disabled={formik.isSubmitting}
                >
                  Cancel
                </Button>
              </SpaceBetween>
            }
          >
            <SpaceBetween size="l">
              <FormField label="Title" errorText={formik.errors.title}>
                <Input name="title" placeholder="Title" />
              </FormField>
            </SpaceBetween>
          </Form>
        )}
      </Formik>
    </Container>
  );
};
