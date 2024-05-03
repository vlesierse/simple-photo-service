import { useEffect, useRef, useState } from "react";
import {
  Button,
  Cards,
  CardsProps,
  Header,
  Link,
  SpaceBetween,
} from "@cloudscape-design/components";
import { useNavigate } from "react-router-dom";
import { get } from "aws-amplify/api";

import { Album } from "./_types";

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const cardDefinition: CardsProps.CardDefinition<Album> = {
  header: (item) => (
    <Link href="#" fontSize="heading-m">
      {item.title}
    </Link>
  ),
};

export const MyAlbums = () => {
  const navigate = useNavigate();
  const [albums, setAlbums] = useState<Album[]>([]);

  const isMounted = useRef<boolean>(false);
  useEffect(() => {
    if (isMounted.current) {
      (async () => {
        const restOperation = get({
          apiName: "AppApi",
          path: "/albums",
        });
        const response = await restOperation.response;
        const json = await response.body.json();
        setAlbums(json as Album[]);
      })();
    }
    isMounted.current = true;
  }, []);

  return (
    <Cards
      items={albums}
      cardDefinition={cardDefinition}
      header={
        <Header
          actions={
            <SpaceBetween size="xs" direction="horizontal">
              <Button
                variant="primary"
                onClick={() => navigate("/albums/create")}
              >
                Create album
              </Button>
            </SpaceBetween>
          }
        >
          My Albums
        </Header>
      }
    />
  );
};
