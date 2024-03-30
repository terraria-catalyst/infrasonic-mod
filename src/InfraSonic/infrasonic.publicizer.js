import {createPublicizer} from "publicizer";

export const publicizer = createPublicizer("InfraSonic");

publicizer.createAssembly("tModLoader").publicizeAll();