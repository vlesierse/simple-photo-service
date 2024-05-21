import { createRef, useEffect, useRef, useState } from "react";
import {
  Button,
  Cards,
  CardsProps,
  Container,
  Header,
} from "@cloudscape-design/components";
import { useParams } from "react-router-dom";
import { get, post } from "aws-amplify/api";

import { Album, Photo, PhotoUpload } from "./_types";

export const ViewAlbum = () => {
  const { id } = useParams();
  const [album, setAlbum] = useState<Album | undefined>();
  const [photos, setPhotos] = useState<Photo[]>([{ id: "" }]);
  const fileInputRef = createRef<HTMLInputElement>();

  const cardDefinition: CardsProps.CardDefinition<Photo> = {
    sections: [
      {
        content: (photo) => (
          <div>
            {photo.url ? (
              <img src={photo.thumbnails?.medium?.url ?? photo.url} />
            ) : (
              <>
                <input
                  ref={fileInputRef}
                  type="file"
                  accept="*.jpg,*.jpeg"
                  hidden
                  onChange={(e) => handleFileUpload(e.target?.files)}
                />
                <Button
                  iconName="add-plus"
                  variant="icon"
                  onClick={() => fileInputRef.current?.click()}
                />
              </>
            )}
          </div>
        ),
      },
    ],
  };

  const handleFileUpload = async (files: FileList | null) => {
    const file = files?.[0];
    if (!file) return;
    setPhotos([{ id: "", url: URL.createObjectURL(file) }, ...photos]);
    /*const restOperation = post({
      apiName: "AppApi",
      path: `/albums/${id}/photos`,
    });
    const response = await restOperation.response;
    const upload = (await response.body.json()) as PhotoUpload;
    await fetch(upload.url, {
      method: "PUT",
      body: file,
    });*/
  };

  const isMounted = useRef<boolean>(false);
  useEffect(() => {
    if (isMounted.current) {
      (async () => {
        const getAlbumOperation = get({
          apiName: "AppApi",
          path: "/albums/" + id,
        });
        const getPhotosOperation = get({
          apiName: "AppApi",
          path: "/albums/" + id + "/photos",
        });
        const albumResponse = await getAlbumOperation.response;
        const photosResponse = await getPhotosOperation.response;
        setAlbum((await albumResponse.body.json()) as Album);
        setPhotos([
          ...((await photosResponse.body.json()) as Photo[]),
          { id: "" },
        ]);
      })();
    }
    isMounted.current = true;
  }, [id]);

  return (
    <Cards
      items={photos}
      cardDefinition={cardDefinition}
      header={<Header>{album?.title}</Header>}
    />
  );
};
