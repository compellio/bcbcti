namespace Compellio.Bcbcti.Services.Serialization.Formats;

public static class DateTimeExtensions
{
    extension(DateTime date)
    {
        /// <summary>
        /// Converts the value of the current DateTime object to its equivalent TAXII timestamp representation (RFC 3339, with microsecond precision, in UTC with Z designation).
        /// </summary>
        /// <see href="https://docs.oasis-open.org/cti/taxii/v2.1/os/taxii-v2.1-os.html#_Toc26285787"/>
        public string ToTaxii()
        {
            return date.ToUniversalTime().ToString("yyy-MM-dd'T'HH:mm:ss.ffffffK");
        }

        /// <summary>
        /// Converts the value of the current DateTime object to its equivalent STIX timestamp representation (RFC 3339, with optional sub-second precision, in UTC with Z designation).
        /// </summary>
        /// <remarks>Using same format as TAXII for consistency.</remarks>
        /// <see href="https://docs.oasis-open.org/cti/stix/v2.1/cs02/stix-v2.1-cs02.html#_ksbm2nost85y"/>
        public string ToStix()
        {
            return date.ToTaxii();
        }
    }
}