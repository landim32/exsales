using Core.Domain;
using Microsoft.Extensions.Logging;
using MonexUp.Domain.Interfaces.Factory;
using MonexUp.Domain.Interfaces.Models;
using MonexUp.Domain.Interfaces.Services;
using MonexUp.DTO.Network;
using MonexUp.DTO.User;
using NAuth.ACL.Interfaces;
using zTools.ACL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MonexUp.Domain.Impl.Services
{
    public class NetworkService : INetworkService
    {

        private readonly INetworkDomainFactory _networkFactory;
        private readonly IUserClient _userClient;
        private readonly IUserNetworkDomainFactory _userNetworkFactory;
        private readonly IUserProfileDomainFactory _userProfileFactory;
        private readonly IProfileService _profileService;
        private readonly IFileClient _fileClient;
        private readonly IInviteTokenSigner _inviteTokenSigner;
        private readonly INetworkInviteDomainFactory _networkInviteFactory;
        private readonly ILogger<NetworkService> _logger;

        public NetworkService(
            IUserClient userClient,
            INetworkDomainFactory networkFactory,
            IUserNetworkDomainFactory userNetworkFactory,
            IUserProfileDomainFactory userProfileFactory,
            IProfileService profileService,
            IFileClient fileClient,
            IInviteTokenSigner inviteTokenSigner,
            INetworkInviteDomainFactory networkInviteFactory,
            ILogger<NetworkService> logger
        )
        {
            _userClient = userClient;
            _networkFactory = networkFactory;
            _userNetworkFactory = userNetworkFactory;
            _userProfileFactory = userProfileFactory;
            _profileService = profileService;
            _fileClient = fileClient;
            _inviteTokenSigner = inviteTokenSigner;
            _networkInviteFactory = networkInviteFactory;
            _logger = logger;
        }

        private string GenerateSlug(INetworkModel md)
        {
            string newSlug;
            int c = 0;
            do
            {
                newSlug = SlugHelper.GerarSlug((!string.IsNullOrEmpty(md.Slug)) ? md.Slug : md.Name);
                if (c > 0)
                {
                    newSlug += c.ToString();
                }
                c++;
            } while (md.ExistSlug(md.NetworkId, newSlug));
            return newSlug;
        }

        public INetworkModel Insert(NetworkInsertInfo network, long userId)
        {
            var model = _networkFactory.BuildNetworkModel();
            if (string.IsNullOrEmpty(network.Name))
            {
                throw new Exception("Name is empty");
            }
            else
            {
                var networkWithName = model.GetByName(network.Name, _networkFactory);
                if (networkWithName != null && networkWithName.NetworkId != model.NetworkId)
                {
                    throw new Exception("Network with this name already registered");
                }
            }
            if (string.IsNullOrEmpty(network.Email))
            {
                throw new Exception("Email is empty");
            }
            else
            {
                if (!EmailValidator.IsValidEmail(network.Email))
                {
                    throw new Exception("Email is not valid");
                }
                var networkWithEmail = model.GetByEmail(network.Email, _networkFactory);
                if (networkWithEmail != null)
                {
                    throw new Exception("Network with email already registered");
                }
            }

            model.Name = network.Name;
            model.Email = network.Email;
            model.Commission = network.Commission;
            model.Plan = network.Plan;
            model.Template = network.Template;
            model.WithdrawalMin = 300;
            model.WithdrawalPeriod = 30;
            model.Status = NetworkStatusEnum.Active;
            model.Slug = GenerateSlug(model);

            var md = model.Insert(_networkFactory);

            // Create the network's default profiles first so the manager
            // membership can reference the "Gerente" profile it belongs to.
            var managerProfile = _userProfileFactory.BuildUserProfileModel();
            managerProfile.NetworkId = md.NetworkId;
            managerProfile.Name = "Gerente";
            managerProfile.Commission = 0;
            managerProfile.Level = 1;
            managerProfile = managerProfile.Insert(_userProfileFactory);

            var sellerProfile = _userProfileFactory.BuildUserProfileModel();
            sellerProfile.NetworkId = md.NetworkId;
            sellerProfile.Name = "Vendedor";
            sellerProfile.Commission = 0;
            sellerProfile.Level = 2;
            sellerProfile.Insert(_userProfileFactory);

            var modelUser = _userNetworkFactory.BuildUserNetworkModel();
            modelUser.NetworkId = md.NetworkId;
            modelUser.UserId = userId;
            modelUser.ProfileId = managerProfile.ProfileId;
            modelUser.Role = DTO.User.UserRoleEnum.NetworkManager;
            modelUser.Status = DTO.User.UserNetworkStatusEnum.Active;
            modelUser.Insert(_userNetworkFactory);

            return md;
        }

        public async Task<INetworkModel> Update(NetworkInfo network, long userId, string token)
        {
            var networkAccess = _userNetworkFactory.BuildUserNetworkModel().Get(network.NetworkId, userId, _userNetworkFactory);

            if (networkAccess == null)
            {
                throw new Exception("Your dont have access to this network");
            }

            if (networkAccess.Role != DTO.User.UserRoleEnum.NetworkManager)
            {
                var user = await _userClient.GetByIdAsync(userId, token);
                if (user == null)
                {
                    throw new Exception("User not found");
                }
                if (!user.IsAdmin)
                {
                    throw new Exception("Your dont have access to this network");
                }
            }

            var model = _networkFactory.BuildNetworkModel();
            if (string.IsNullOrEmpty(network.Name))
            {
                throw new Exception("Name is empty");
            }
            else
            {
                var networkWithName = model.GetByName(network.Name, _networkFactory);
                if (networkWithName != null && networkWithName.NetworkId != network.NetworkId)
                {
                    throw new Exception("Network with this name already registered");
                }
            }
            if (string.IsNullOrEmpty(network.Email))
            {
                throw new Exception("Email is empty");
            }
            else
            {
                if (!EmailValidator.IsValidEmail(network.Email))
                {
                    throw new Exception("Email is not valid");
                }
                var networkWithEmail = model.GetByEmail(network.Email, _networkFactory);
                if (networkWithEmail != null && networkWithEmail.NetworkId != network.NetworkId)
                {
                    throw new Exception("Network with email already registered");
                }
            }

            // Preserve store IDs that aren't in the user-editable form. The
            // repository does a full-row ModelToDb copy, so leaving them
            // unset would wipe ProxyPayStore/LofnStore state from the DB.
            var existing = _networkFactory.BuildNetworkModel()
                .GetById(network.NetworkId, _networkFactory);

            model.NetworkId = network.NetworkId;
            model.Name = network.Name;
            model.Slug = network.Slug;
            model.Template = network.Template;
            model.Image = network.ImageUrl;
            model.Email = network.Email;
            model.Commission = network.Commission;
            model.Plan = network.Plan;
            model.WithdrawalMin = network.WithdrawalMin;
            model.WithdrawalPeriod = network.WithdrawalPeriod;
            model.Status = network.Status;
            model.LofnStoreId = existing?.LofnStoreId;
            model.ProxyPayStoreId = existing?.ProxyPayStoreId;
            model.ProxyPayClientId = existing?.ProxyPayClientId;
            model.Slug = GenerateSlug(model);

            var md = model.Update(_networkFactory);

            return md;
        }
        public IList<INetworkModel> ListByStatus(NetworkStatusEnum status)
        {
            return _networkFactory.BuildNetworkModel().ListByStatus(status, _networkFactory).ToList();
        }
        public IList<IUserNetworkModel> ListByUser(long userId)
        {
            return _userNetworkFactory.BuildUserNetworkModel().ListByUser(userId, _userNetworkFactory).ToList();
        }

        public IList<IUserNetworkModel> ListByNetwork(long networkId, bool includeAllStatuses)
        {
            return _userNetworkFactory.BuildUserNetworkModel().ListByNetwork(networkId, includeAllStatuses, _userNetworkFactory).ToList();
        }

        public INetworkModel GetById(long networkId)
        {
            return _networkFactory.BuildNetworkModel().GetById(networkId, _networkFactory);
        }

        public INetworkModel GetBySlug(string slug)
        {
            return _networkFactory.BuildNetworkModel().GetBySlug(slug, _networkFactory);
        }

        public IUserNetworkModel GetUserNetwork(long networkId, long userId)
        {
            return _userNetworkFactory.BuildUserNetworkModel().Get(networkId, userId, _userNetworkFactory);
        }

        public async Task<UserNetworkInfo> GetUserNetworkInfo(IUserNetworkModel model, string token)
        {
            if (model == null)
            {
                return null;
            }

            // Public surfaces (storefront, seller landing) hit this without a
            // bearer token. NAuth refuses unauthenticated calls, so we guard
            // the User fetch and degrade gracefully instead of 500'ing the
            // whole request.
            NAuth.DTO.User.UserInfo userInfo = null;
            if (!string.IsNullOrWhiteSpace(token))
            {
                try
                {
                    userInfo = await _userClient.GetByIdAsync(model.UserId, token);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "NAuth GetByIdAsync failed for userId={UserId} — returning UserNetworkInfo without user details.", model.UserId);
                    userInfo = null;
                }
            }

            return new UserNetworkInfo
            {
                NetworkId = model.NetworkId,
                UserId = model.UserId,
                ProfileId = model.ProfileId,
                ReferrerId = model.ReferrerId,
                Role = model.Role,
                Status = model.Status,
                Invited = model.InvitedAt.HasValue,
                Network = await GetNetworkInfo(model.GetNetwork(_networkFactory)),
                User = userInfo,
                Profile = _profileService.GetUserProfileInfo(
                    _userProfileFactory.BuildUserProfileModel()
                    .GetById(model.ProfileId.GetValueOrDefault(), _userProfileFactory)
                )

            };
        }

        public async Task<NetworkInfo> GetNetworkInfo(INetworkModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new NetworkInfo
            {
                NetworkId = model.NetworkId,
                Name = model.Name,
                Slug = model.Slug,
                Template = model.Template,
                ImageUrl = await GetImageUrl(model),
                Email = model.Email,
                Plan = model.Plan,
                Commission = model.Commission,
                WithdrawalMin = model.WithdrawalMin,
                WithdrawalPeriod = model.WithdrawalPeriod,
                QtdyUsers = _userNetworkFactory.BuildUserNetworkModel().GetQtdyUserByNetwork(model.NetworkId),
                MaxUsers = model.MaxQtdyUserByNetwork(),
                Status = model.Status,
                LofnStoreId = model.LofnStoreId,
                ProxyPayStoreId = model.ProxyPayStoreId,
                ProxyPayClientId = model.ProxyPayClientId
            };
        }

        /// <summary>
        /// Resolve a URL da imagem da rede. A imagem é opcional: se não houver arquivo
        /// gravado ou se o storage não encontrar o objeto (404), devolve null em vez de
        /// derrubar a listagem inteira de redes.
        /// </summary>
        private async Task<string> GetImageUrl(INetworkModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Image))
            {
                return null;
            }

            try
            {
                return await _fileClient.GetFileUrlAsync("monexup", model.Image);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "FileClient GetFileUrlAsync failed for networkId={NetworkId}, image={Image} — returning NetworkInfo without ImageUrl.", model.NetworkId, model.Image);
                return null;
            }
        }

        public void RequestAccess(long networkId, long userId, long? referrerId)
        {
            CreatePendingMembership(networkId, userId, referrerId);
        }

        /// <summary>
        /// UTC "now" with Kind=Unspecified. Npgsql refuses to write a Kind=Utc
        /// DateTime into a `timestamp without time zone` column, which is the type
        /// of invited_at / consumed_at. Mirrors ProductLinkRepository.
        /// </summary>
        private static DateTime UtcNowForDb()
            => DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        /// <summary>
        /// Creates a WaitForApproval membership (lowest profile, Seller role) with
        /// the given referrer. Shared by self-service RequestAccess and the invite flows.
        /// <paramref name="invitedAt"/> is left null by RequestAccess — that is what
        /// distinguishes "Convidado" from "solicitou acesso" on /admin/teams.
        /// </summary>
        private void CreatePendingMembership(long networkId, long userId, long? referrerId, DateTime? invitedAt = null)
        {
            var profiles = _userProfileFactory.BuildUserProfileModel().ListByNetwork(networkId, _userProfileFactory);

            var lowerProfile = profiles.OrderByDescending(x => x.Level).FirstOrDefault();
            if (lowerProfile == null)
            {
                throw new Exception("Lower profile not found");
            }

            var model = _userNetworkFactory.BuildUserNetworkModel();
            model.NetworkId = networkId;
            model.UserId = userId;
            model.ProfileId = lowerProfile.ProfileId;
            model.Role = DTO.User.UserRoleEnum.Seller;
            model.Status = DTO.User.UserNetworkStatusEnum.WaitForApproval;
            model.ReferrerId = referrerId;
            model.InvitedAt = invitedAt;

            model.Insert(_userNetworkFactory);
        }

        /// <summary>
        /// Authorizes that <paramref name="managerId"/> may manage <paramref name="networkId"/>
        /// (NetworkManager of the network, or a platform admin). Mirrors ValidateAccess
        /// without requiring a pre-existing target membership.
        /// </summary>
        private async Task ValidateManager(long networkId, long managerId, string token)
        {
            var networkAccess = _userNetworkFactory.BuildUserNetworkModel().Get(networkId, managerId, _userNetworkFactory);
            if (networkAccess == null)
            {
                throw new UnauthorizedAccessException("Your dont have access to this network");
            }

            if (networkAccess.Role != DTO.User.UserRoleEnum.NetworkManager)
            {
                var user = await _userClient.GetByIdAsync(managerId, token);
                if (user == null || !user.IsAdmin)
                {
                    throw new UnauthorizedAccessException("Your dont have access to this network");
                }
            }
        }

        public async Task<InviteResultInfo> InviteByEmail(long networkId, string email, long inviterUserId, string token)
        {
            if (string.IsNullOrWhiteSpace(email) || !EmailValidator.IsValidEmail(email))
            {
                return new InviteResultInfo { Sucesso = false, MensagemErro = "E-mail inválido." };
            }

            await ValidateManager(networkId, inviterUserId, token);

            var network = _networkFactory.BuildNetworkModel().GetById(networkId, _networkFactory);
            if (network == null)
            {
                return new InviteResultInfo { Sucesso = false, MensagemErro = "Rede não encontrada." };
            }

            // NAuth GetByEmailAsync throws (EnsureSuccessStatusCode) when the
            // email has no account — treat any failure as "no account" and log.
            NAuth.DTO.User.UserInfo invitee = null;
            try
            {
                invitee = await _userClient.GetByEmailAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "NAuth GetByEmailAsync found no account for {Email} — treating as no-account invite.", email);
                invitee = null;
            }

            if (invitee == null)
            {
                // No account → no membership row is possible (the PK needs a user id),
                // so the invite is persisted on its own table to stay visible on
                // /admin/teams until the invitee signs up or the manager cancels.
                var normalizedEmail = email.Trim().ToLowerInvariant();
                var invite = _networkInviteFactory.BuildNetworkInviteModel()
                    .GetPending(networkId, normalizedEmail, _networkInviteFactory);

                if (invite == null)
                {
                    var newInvite = _networkInviteFactory.BuildNetworkInviteModel();
                    newInvite.NetworkId = networkId;
                    newInvite.Email = normalizedEmail;
                    newInvite.InviterUserId = inviterUserId;
                    newInvite.Status = NetworkInviteStatusEnum.Pending;
                    invite = newInvite.Insert(_networkInviteFactory);
                }

                var newToken = _inviteTokenSigner.Sign(networkId, inviterUserId, 0, false, invite.InviteId);
                return new InviteResultInfo
                {
                    Sucesso = true,
                    HasAccount = false,
                    AlreadyMember = false,
                    Token = newToken,
                    NetworkSlug = network.Slug
                };
            }

            if (invitee.UserId == inviterUserId)
            {
                return new InviteResultInfo { Sucesso = false, MensagemErro = "Você não pode convidar a si mesmo." };
            }

            var existing = _userNetworkFactory.BuildUserNetworkModel().Get(networkId, invitee.UserId, _userNetworkFactory);
            var alreadyMember = false;

            if (existing != null &&
                (existing.Status == DTO.User.UserNetworkStatusEnum.Active
                 || existing.Status == DTO.User.UserNetworkStatusEnum.WaitForApproval))
            {
                // Idempotent: already active/pending — surface state, no duplicate.
                alreadyMember = true;
            }
            else if (existing != null)
            {
                // Inactive/Blocked → reactivate to pending with the new referrer.
                existing.Status = DTO.User.UserNetworkStatusEnum.WaitForApproval;
                existing.ReferrerId = inviterUserId;
                existing.InvitedAt = UtcNowForDb();
                existing.Update(_userNetworkFactory);
            }
            else
            {
                // Create the pending membership at invite time (per FR-007/FR-012).
                CreatePendingMembership(networkId, invitee.UserId, inviterUserId, UtcNowForDb());
            }

            var token2 = _inviteTokenSigner.Sign(networkId, inviterUserId, invitee.UserId, true);
            return new InviteResultInfo
            {
                Sucesso = true,
                HasAccount = true,
                AlreadyMember = alreadyMember,
                Token = token2,
                NetworkSlug = network.Slug
            };
        }

        public Task JoinFromInvite(long joinerUserId, string inviteToken)
        {
            if (!_inviteTokenSigner.TryVerify(inviteToken, out var payload))
            {
                throw new Exception("Convite inválido.");
            }

            var existing = _userNetworkFactory.BuildUserNetworkModel().Get(payload.NetworkId, joinerUserId, _userNetworkFactory);
            if (existing != null &&
                (existing.Status == DTO.User.UserNetworkStatusEnum.Active
                 || existing.Status == DTO.User.UserNetworkStatusEnum.WaitForApproval))
            {
                // Idempotent — already active/pending.
                ConsumeInvite(payload.InviteId, payload.NetworkId, joinerUserId);
                return Task.CompletedTask;
            }

            if (existing != null)
            {
                existing.Status = DTO.User.UserNetworkStatusEnum.WaitForApproval;
                existing.ReferrerId = payload.InviterUserId;
                existing.InvitedAt = UtcNowForDb();
                existing.Update(_userNetworkFactory);
                ConsumeInvite(payload.InviteId, payload.NetworkId, joinerUserId);
                return Task.CompletedTask;
            }

            CreatePendingMembership(payload.NetworkId, joinerUserId, payload.InviterUserId, UtcNowForDb());
            ConsumeInvite(payload.InviteId, payload.NetworkId, joinerUserId);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Marks the no-account invite row as Accepted once its invitee has signed
        /// up. No-op for legacy 4-segment tokens (inviteId 0 — nothing was ever
        /// persisted for those) and for invites already consumed or cancelled.
        /// </summary>
        private void ConsumeInvite(long inviteId, long networkId, long joinerUserId)
        {
            if (inviteId <= 0)
            {
                return;
            }

            var invite = _networkInviteFactory.BuildNetworkInviteModel()
                .GetById(inviteId, _networkInviteFactory);

            if (invite == null
                || invite.NetworkId != networkId
                || invite.Status != NetworkInviteStatusEnum.Pending)
            {
                return;
            }

            invite.Status = NetworkInviteStatusEnum.Accepted;
            invite.ConsumedAt = UtcNowForDb();
            invite.ConsumedUserId = joinerUserId;
            invite.Update(_networkInviteFactory);
        }

        public async Task<InviteDetailInfo> GetInviteDetail(long callerUserId, string inviteToken, string token)
        {
            if (!_inviteTokenSigner.TryVerify(inviteToken, out var payload))
            {
                return new InviteDetailInfo { Sucesso = false, MensagemErro = "Convite inválido." };
            }

            var network = _networkFactory.BuildNetworkModel().GetById(payload.NetworkId, _networkFactory);

            string inviterName = null;
            try
            {
                var inviter = await _userClient.GetByIdAsync(payload.InviterUserId, token);
                inviterName = inviter?.Name;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "NAuth GetByIdAsync failed for inviter {InviterId} — returning invite detail without inviter name.", payload.InviterUserId);
            }

            var existing = _userNetworkFactory.BuildUserNetworkModel().Get(payload.NetworkId, callerUserId, _userNetworkFactory);

            return new InviteDetailInfo
            {
                Sucesso = true,
                NetworkId = payload.NetworkId,
                NetworkName = network?.Name,
                InviterName = inviterName,
                TargetUserId = payload.TargetUserId,
                IsForCurrentUser = payload.TargetUserId == callerUserId,
                AlreadyActiveMember = existing != null && existing.Status == DTO.User.UserNetworkStatusEnum.Active
            };
        }

        public Task AcceptInvite(long callerUserId, string inviteToken)
        {
            if (!_inviteTokenSigner.TryVerify(inviteToken, out var payload))
            {
                throw new Exception("Convite inválido.");
            }
            if (payload.TargetUserId != callerUserId)
            {
                throw new UnauthorizedAccessException("Este convite não é para a sua conta.");
            }

            var existing = _userNetworkFactory.BuildUserNetworkModel().Get(payload.NetworkId, callerUserId, _userNetworkFactory);
            if (existing == null)
            {
                // Pending row should already exist from invite time; recreate idempotently if missing.
                CreatePendingMembership(payload.NetworkId, callerUserId, payload.InviterUserId, UtcNowForDb());
            }
            else if (existing.Status == DTO.User.UserNetworkStatusEnum.Inactive)
            {
                existing.Status = DTO.User.UserNetworkStatusEnum.WaitForApproval;
                existing.ReferrerId = payload.InviterUserId;
                existing.InvitedAt = UtcNowForDb();
                existing.Update(_userNetworkFactory);
            }
            // Active/WaitForApproval → no-op; still requires manager approval.
            return Task.CompletedTask;
        }

        /// <summary>
        /// Pending no-account invites of a network, for the /admin/teams list.
        /// Manager-only: the payload carries invitee e-mail addresses, so this is
        /// never exposed through the public listByNetwork.
        /// </summary>
        public async Task<IList<NetworkInviteInfo>> ListPendingInvites(long networkId, long managerId, string token)
        {
            await ValidateManager(networkId, managerId, token);

            var network = _networkFactory.BuildNetworkModel().GetById(networkId, _networkFactory);
            if (network == null)
            {
                throw new Exception("Rede não encontrada.");
            }

            var invites = _networkInviteFactory.BuildNetworkInviteModel()
                .ListPendingByNetwork(networkId, _networkInviteFactory);

            var nameCache = new Dictionary<long, string>();
            var result = new List<NetworkInviteInfo>();
            foreach (var invite in invites)
            {
                result.Add(new NetworkInviteInfo
                {
                    InviteId = invite.InviteId,
                    NetworkId = invite.NetworkId,
                    Email = invite.Email,
                    InviterUserId = invite.InviterUserId,
                    InviterName = await ResolveName(invite.InviterUserId, token, nameCache),
                    Status = invite.Status,
                    CreatedAt = invite.CreatedAt,
                    // Re-signed on read — the token itself is never stored.
                    Token = _inviteTokenSigner.Sign(invite.NetworkId, invite.InviterUserId, 0, false, invite.InviteId),
                    NetworkSlug = network.Slug
                });
            }

            return result;
        }

        /// <summary>Manager-only cancellation of a pending no-account invite.</summary>
        public async Task CancelInvite(long inviteId, long managerId, string token)
        {
            var invite = _networkInviteFactory.BuildNetworkInviteModel()
                .GetById(inviteId, _networkInviteFactory);

            if (invite == null)
            {
                throw new Exception("Convite não encontrado.");
            }

            await ValidateManager(invite.NetworkId, managerId, token);

            if (invite.Status != NetworkInviteStatusEnum.Pending)
            {
                throw new Exception("Este convite não está pendente.");
            }

            invite.Status = NetworkInviteStatusEnum.Cancelled;
            invite.Update(_networkInviteFactory);
        }

        public Task DeclineInvite(long callerUserId, string inviteToken)
        {
            if (!_inviteTokenSigner.TryVerify(inviteToken, out var payload))
            {
                throw new Exception("Convite inválido.");
            }
            if (payload.TargetUserId != callerUserId)
            {
                throw new UnauthorizedAccessException("Este convite não é para a sua conta.");
            }

            var existing = _userNetworkFactory.BuildUserNetworkModel().Get(payload.NetworkId, callerUserId, _userNetworkFactory);
            if (existing != null && existing.Status == DTO.User.UserNetworkStatusEnum.WaitForApproval)
            {
                existing.Status = DTO.User.UserNetworkStatusEnum.Inactive;
                existing.Update(_userNetworkFactory);
            }
            return Task.CompletedTask;
        }

        // ------------------------------------------------------------------
        // Hierarchy tree (feature 010) — bounded 3 up / 3 down from the caller,
        // built off UserNetwork.ReferrerId, all statuses, cycle-safe.
        // ------------------------------------------------------------------
        private const int HierarchyMaxDepth = 3;

        public async Task<HierarchyInfo> BuildHierarchy(long networkId, long userId, string token)
        {
            var self = _userNetworkFactory.BuildUserNetworkModel().Get(networkId, userId, _userNetworkFactory);
            if (self == null)
            {
                // Caller is not a member of this network — no tree.
                return null;
            }

            var nameCache = new Dictionary<long, string>();
            var visited = new HashSet<long> { userId };

            var current = await ToHierarchyNode(self, token, nameCache);

            // Ascend the referrer chain, up to HierarchyMaxDepth (immediate referrer first).
            var ancestors = new List<HierarchyNodeInfo>();
            var cursor = self;
            for (var i = 0; i < HierarchyMaxDepth; i++)
            {
                if (!cursor.ReferrerId.HasValue) break;
                var referrerId = cursor.ReferrerId.Value;
                if (visited.Contains(referrerId)) break; // cycle guard
                var referrer = _userNetworkFactory.BuildUserNetworkModel().Get(networkId, referrerId, _userNetworkFactory);
                if (referrer == null) break; // referrer not in this network — chain ends
                visited.Add(referrerId);
                ancestors.Add(await ToHierarchyNode(referrer, token, nameCache));
                cursor = referrer;
            }

            var descendants = await BuildDescendants(networkId, userId, HierarchyMaxDepth, token, nameCache, visited);

            return new HierarchyInfo
            {
                NetworkId = networkId,
                Current = current,
                Ancestors = ancestors,
                Descendants = descendants
            };
        }

        private async Task<IList<HierarchyNodeInfo>> BuildDescendants(
            long networkId, long parentUserId, int remainingDepth,
            string token, Dictionary<long, string> nameCache, HashSet<long> visited)
        {
            var result = new List<HierarchyNodeInfo>();
            if (remainingDepth <= 0) return result;

            var children = _userNetworkFactory.BuildUserNetworkModel()
                .GetByReferrer(networkId, parentUserId, _userNetworkFactory);

            foreach (var child in children)
            {
                if (visited.Contains(child.UserId)) continue; // cycle / duplicate guard
                visited.Add(child.UserId);
                var node = await ToHierarchyNode(child, token, nameCache);
                node.Children = await BuildDescendants(networkId, child.UserId, remainingDepth - 1, token, nameCache, visited);
                result.Add(node);
            }
            return result;
        }

        private async Task<HierarchyNodeInfo> ToHierarchyNode(IUserNetworkModel model, string token, Dictionary<long, string> nameCache)
        {
            string profileName = null;
            if (model.ProfileId.HasValue && model.ProfileId.Value > 0)
            {
                profileName = _userProfileFactory.BuildUserProfileModel()
                    .GetById(model.ProfileId.Value, _userProfileFactory)?.Name;
            }

            return new HierarchyNodeInfo
            {
                UserId = model.UserId,
                Name = await ResolveName(model.UserId, token, nameCache),
                ProfileName = profileName,
                Role = model.Role,
                Status = model.Status,
                Children = new List<HierarchyNodeInfo>()
            };
        }

        private async Task<string> ResolveName(long userId, string token, Dictionary<long, string> nameCache)
        {
            if (nameCache.TryGetValue(userId, out var cached))
            {
                return cached;
            }

            string name = null;
            if (!string.IsNullOrWhiteSpace(token))
            {
                try
                {
                    var user = await _userClient.GetByIdAsync(userId, token);
                    name = user?.Name;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "NAuth GetByIdAsync failed for userId={UserId} — row rendered without name.", userId);
                }
            }

            nameCache[userId] = name;
            return name;
        }

        private async Task ValidateAccess(long networkId, long userId, long managerId, string token)
        {
            var userNetwork = _userNetworkFactory.BuildUserNetworkModel().Get(networkId, userId, _userNetworkFactory);
            if (userNetwork == null)
            {
                throw new Exception("Access is not required");
            }

            var networkAccess = _userNetworkFactory.BuildUserNetworkModel().Get(networkId, managerId, _userNetworkFactory);

            if (networkAccess == null)
            {
                throw new Exception("Your dont have access to this network");
            }

            if (networkAccess.Role != DTO.User.UserRoleEnum.NetworkManager)
            {
                var user = await _userClient.GetByIdAsync(userId, token);
                if (user == null)
                {
                    throw new Exception("User not found");
                }
                if (!user.IsAdmin)
                {
                    throw new Exception("Your dont have access to this network");
                }
            }
        }
        public async Task ChangeStatus(long networkId, long userId, UserNetworkStatusEnum status, long managerId, string token)
        {
            await ValidateAccess(networkId, userId, managerId, token);

            var userNetwork = _userNetworkFactory.BuildUserNetworkModel().Get(networkId, userId, _userNetworkFactory);
            userNetwork.Status = status;
            userNetwork.Update(_userNetworkFactory);
        }

        public async Task<bool> Promote(long networkId, long userId, long managerId, string token)
        {
            await ValidateAccess(networkId, userId, managerId, token);

            return _userNetworkFactory.BuildUserNetworkModel().Promote(networkId, userId);
        }

        public async Task<bool> Demote(long networkId, long userId, long managerId, string token)
        {
            await ValidateAccess(networkId, userId, managerId, token);

            return _userNetworkFactory.BuildUserNetworkModel().Demote(networkId, userId);
        }
    }
}
