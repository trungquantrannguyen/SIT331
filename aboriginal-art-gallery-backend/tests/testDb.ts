import dotenv from "dotenv";
import mongoose from "mongoose";

dotenv.config();

export const connectTestDb = async (): Promise<void> => {
  const mongoTestUri = process.env.MONGO_TEST_URI;

  if (!mongoTestUri) {
    throw new Error("MONGO_TEST_URI is missing from environment variables.");
  }

  if (mongoose.connection.readyState === 0) {
    await mongoose.connect(mongoTestUri);
  }
};

export const clearTestDb = async (): Promise<void> => {
  const collections = mongoose.connection.collections;

  for (const key of Object.keys(collections)) {
    await collections[key].deleteMany({});
  }
};

export const closeTestDb = async (): Promise<void> => {
  await mongoose.connection.close();
};
