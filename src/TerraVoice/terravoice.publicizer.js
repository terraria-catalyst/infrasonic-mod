import {createPublicizer} from "publicizer";

export const publicizer = createPublicizer("TerraVoice");

publicizer.createAssembly("tModLoader").publicizeAll();