import { useEffect, useRef, useState } from "react";
import {
  Button,
  Cards,
  Container,
  Header,
  SpaceBetween,
} from "@cloudscape-design/components";
import { useNavigate, useParams } from "react-router-dom";
import { get } from "aws-amplify/api";

import { Album } from "./_types";

export const ViewAlbum = () => {
  const { id } = useParams();
  const [album, setAlbum] = useState<Album | undefined>();

  const isMounted = useRef<boolean>(false);
  useEffect(() => {
    if (isMounted.current) {
      (async () => {
        const restOperation = get({
          apiName: "AppApi",
          path: "/albums/" + id,
        });
        const response = await restOperation.response;
        const json = await response.body.json();
        setAlbum(json as Album);
      })();
    }
    isMounted.current = true;
  }, [id]);

  return <Container header={<Header variant="h2">{album?.title}</Header>} />;
};
