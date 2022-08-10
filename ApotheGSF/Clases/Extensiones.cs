using System.Globalization;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace ApotheGSF.Clases
{
    public static class Extensiones
    {
		/// <summary>
		/// Busca en los datos del ClaimPrincipal el identificador que representa el ID del usuario conectado
		/// </summary>
		/// <param name="principal"></param>
		/// <returns></returns>
		public static string GetUserID(this ClaimsPrincipal principal)
		{
			if (principal == null)
				throw new ArgumentNullException(nameof(principal));

			return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		}

		/// <summary>
		/// Busca en el claimPrincipal el username del usuario
		/// </summary>
		/// <param name="principal"></param>
		/// <returns></returns>
		public static string GetUserName(this ClaimsPrincipal principal)
		{
			if (principal == null)
				throw new ArgumentNullException(nameof(principal));
			return principal.FindFirst(ClaimTypes.Name)?.Value;
		}

		/// <summary>
		/// Verifica en el listado de claims(permiso) que tiene el usuario si existe el permiso 
		/// que se especifica dentro del paréntesis
		/// </summary>
		/// <param name="claim">El claimPrincipal del usuario.</param>
		/// <param name="permiso">El número del permiso a verificar.</param>
		/// <returns></returns>
		public static bool CheckPermiso(this ClaimsPrincipal claim, string permiso)
		{
			foreach (var item in claim.Claims.Where(x => x.Type.Equals("Permiso")))
			{
				if (item.Value.Equals("_@_"))
					return true;

				if (item.Value.Equals(permiso))
					return true;
			}
			return false;
		}
		public static string GetNombre(this ClaimsPrincipal principal)
		{
			if (principal == null)
				throw new ArgumentNullException(nameof(principal));
			return principal.FindFirst(c => c.Type == "Nombre")?.Value;
		}

		public static int ToInt(this string valor)
		{
			int.TryParse(valor, out int dato);
			return dato;
		}
		public static bool IsValidEmail(this string email)
		{
			if (string.IsNullOrWhiteSpace(email))
				return false;

			try
			{
				// Normalize the domain
				email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
									  RegexOptions.None, TimeSpan.FromMilliseconds(200));

				// Examines the domain part of the email and normalizes it.
				string DomainMapper(Match match)
				{
					// Use IdnMapping class to convert Unicode domain names.
					var idn = new IdnMapping();

					// Pull out and process domain name (throws ArgumentException on invalid)
					var domainName = idn.GetAscii(match.Groups[2].Value);

					return match.Groups[1].Value + domainName;
				}
			}
			catch (RegexMatchTimeoutException)
			{
				return false;
			}
			catch// (ArgumentException e)
			{
				return false;
			}

			try
			{
				return Regex.IsMatch(email,
					@"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
					@"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
					RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
			}
			catch (RegexMatchTimeoutException)
			{
				return false;
			}
		}

		public static bool ValidarFechaNacimiento(this DateTime fechaNacimiento)
        {
			int yearMin = fechaNacimiento.Year - DateTime.Now.AddYears(-18).Year;
			int yearMax = fechaNacimiento.Year - DateTime.Now.AddYears(-70).Year;
			if (yearMin > 0 || yearMax < 0)
            {
				return false;
            }

			return true;
		}

		public static bool Contains(this string text, string[] keywods, bool ignoreCase = false)
		{
			bool found = false;
			text = ignoreCase ? text.ToLower() : text;

			foreach (var keyword in keywods)
			{
				var keywordTemp = ignoreCase ? keyword.ToLower() : keyword;
				if (text.Contains(keywordTemp.Trim()))
				{
					found = true;
					break;
				}
			}
			return found;
		}
	}
}
