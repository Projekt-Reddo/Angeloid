namespace Angeloid.Services
{
    public static class AutoAnimeProcessing
    {
        private static bool isProcessing = false;
        public static bool getIsProcessing() {
            return isProcessing;
        }
        public static void setDone() {
            isProcessing = false;
        }
        public static void setProcessing() {
            isProcessing = true;
        }
    }
}