import { useEffect, useRef } from "react";
import YouTube from "react-youtube";

const SafeYouTubePlayer = ({ videoId, opts, className }) => {
    const playerRef = useRef(null);
    const mounted = useRef(true);

    useEffect(() => {
        mounted.current = true;

        return () => {
            mounted.current = false;
            if (playerRef.current && typeof playerRef.current.destroy === "function") {
                playerRef.current.destroy();
            }
        };
    }, []);

    const onReady = (event) => {
        if (!mounted.current) return; 
        playerRef.current = event.target;
    };

    return <YouTube videoId={videoId} opts={opts} className={className} onReady={onReady} />;
};

export default SafeYouTubePlayer;
