export type Album = {
  id: string;
  title: string;
  photos: Photo[];
};

export type Photo = {
  id: string;
  url?: string;
  thumbnails?: Thumbnails;
  metadata?: PhotoMetadata;
};

export type PhotoMetadata = {
  width: number;
  height: number;
  labels: Label[];
  explicitContent: boolean;
};

export type Label = {
  name: string;
  confidence: number;
};

export type Thumbnails = {
  small?: Thumbnail;
  medium?: Thumbnail;
  large?: Thumbnail;
};

export type Thumbnail = {
  url: string;
};

export type PhotoUpload = {
  id: string;
  url: string;
};
