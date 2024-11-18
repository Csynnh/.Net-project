using infrastructure.DataModels;
using infrastructure.Repositories;

namespace service;

public class OtpService
{
  private readonly EmailService _emailService;
  private readonly OtpRepository _otpRepository;
  private readonly UserRepository _userRepository;

  public OtpService(EmailService emailService, OtpRepository otpRepository, UserRepository userRepository)
  {
    _emailService = emailService;
    _otpRepository = otpRepository;
    _userRepository = userRepository;
  }

  public async Task GenerateAndSendOtpAsync(string email)
  {
    try
    {
      var user = await _userRepository.GetUserByEmailAsync(email);
      if (user == null)
      {
        throw new Exception($"Email {email} not found");
      }
      var otp = new Random().Next(100000, 999999).ToString();
      await _otpRepository.SaveOtpAsync(email, otp);
      await _emailService.SendOtpEmailAsync(email, otp);
    }
    catch (Exception ex)
    {
      throw new Exception($"Failed to generate and send OTP: {ex.Message}");
    }
  }

  public async Task<bool> ValidateOtp(string email, string otp)
  {
    try
    {
      OtpRessponse otpStorage = await _otpRepository.GetOtpByEmailAsync(email);
      DateTime now = DateTime.UtcNow;
      bool isCorrectOtp = otpStorage.Otp == otp;
      bool isOtpExpired = (now - otpStorage.Created_At).TotalMinutes > 5;
      if (!isCorrectOtp)
      {
        throw new Exception("OTP is incorrect");
      }
      else if (isOtpExpired)
      {
        throw new Exception("OTP expired");
      }
      return true;
    }
    catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }
  }
}
