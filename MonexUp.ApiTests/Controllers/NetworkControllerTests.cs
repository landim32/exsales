using System.Text.Json.Serialization;
using FluentAssertions;
using Flurl.Http;
using MonexUp.ApiTests.Fixtures;
using MonexUp.ApiTests.Helpers;
using MonexUp.DTO.Network;
using MonexUp.DTO.User;

namespace MonexUp.ApiTests.Controllers
{
    [Collection("ApiTests")]
    public class NetworkControllerTests
    {
        private readonly ApiTestFixture _fixture;

        public NetworkControllerTests(ApiTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ListAll_ShouldReturnOk()
        {
            var response = await _fixture.CreateAnonymousRequest("/network/listAll")
                .AllowAnyHttpStatus()
                .GetAsync();

            response.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetBySlug_WithInvalidSlug_ShouldReturnSuccessOrNoContent()
        {
            var response = await _fixture.CreateAnonymousRequest("/network/getBySlug/non-existent-slug-999")
                .AllowAnyHttpStatus()
                .GetAsync();

            response.StatusCode.Should().BeOneOf(200, 204);
        }

        [Fact]
        public async Task Insert_WithoutAuth_ShouldReturn401()
        {
            var param = TestDataHelper.CreateNetworkInsertInfo();

            var response = await _fixture.CreateAnonymousRequest("/network/insert")
                .AllowAnyHttpStatus()
                .PostJsonAsync(param);

            response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task Insert_WithAuth_ShouldReturnOkAndPersistNetwork()
        {
            var param = TestDataHelper.CreateNetworkInsertInfo();

            var response = await _fixture.CreateAuthenticatedRequest("/network/insert")
                .AllowAnyHttpStatus()
                .PostJsonAsync(param);

            response.StatusCode.Should().Be(200, "authenticated request with valid payload should create the network");

            var created = await response.GetJsonAsync<NetworkInfo>();
            created.Should().NotBeNull();
            created.NetworkId.Should().BeGreaterThan(0);
            created.Name.Should().Be(param.Name);
            created.Email.Should().Be(param.Email);
            created.Plan.Should().Be(param.Plan);
            created.Status.Should().Be(NetworkStatusEnum.Active);
            created.Slug.Should().NotBeNullOrEmpty();
            created.LofnStoreId.Should().BeNull("Lofn store is provisioned lazily on first product create, not on network insert");
        }

        [Fact]
        public async Task Insert_WithAuth_ShouldGenerateUniqueSlugPerNetwork()
        {
            var first = TestDataHelper.CreateNetworkInsertInfo();
            var firstResponse = await _fixture.CreateAuthenticatedRequest("/network/insert")
                .AllowAnyHttpStatus()
                .PostJsonAsync(first);
            firstResponse.StatusCode.Should().Be(200);
            var firstBody = await firstResponse.GetJsonAsync<NetworkInfo>();

            var second = TestDataHelper.CreateNetworkInsertInfo();
            var secondResponse = await _fixture.CreateAuthenticatedRequest("/network/insert")
                .AllowAnyHttpStatus()
                .PostJsonAsync(second);
            secondResponse.StatusCode.Should().Be(200);
            var secondBody = await secondResponse.GetJsonAsync<NetworkInfo>();

            secondBody.NetworkId.Should().NotBe(firstBody.NetworkId);
            secondBody.Slug.Should().NotBe(firstBody.Slug, "each insert must produce a unique slug");
        }

        [Fact]
        public async Task Insert_WithEmptyName_ShouldReturnNon200()
        {
            var param = TestDataHelper.CreateNetworkInsertInfo();
            param.Name = string.Empty;

            var response = await _fixture.CreateAuthenticatedRequest("/network/insert")
                .AllowAnyHttpStatus()
                .PostJsonAsync(param);

            response.StatusCode.Should().NotBe(200, "blank name violates the slug/name constraint and must not create a network");
        }

        [Fact]
        public async Task Update_WithoutAuth_ShouldReturn401()
        {
            var param = TestDataHelper.CreateNetworkInfo();

            var response = await _fixture.CreateAnonymousRequest("/network/update")
                .AllowAnyHttpStatus()
                .PostJsonAsync(param);

            response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task Update_WithAuth_ShouldNotReturn401()
        {
            var param = TestDataHelper.CreateNetworkInfo();

            var response = await _fixture.CreateAuthenticatedRequest("/network/update")
                .AllowAnyHttpStatus()
                .PostJsonAsync(param);

            response.StatusCode.Should().NotBe(401, "authenticated request should not be rejected");
        }

        [Fact]
        public async Task ListByUser_WithoutAuth_ShouldReturn401()
        {
            var response = await _fixture.CreateAnonymousRequest("/network/listByUser")
                .AllowAnyHttpStatus()
                .GetAsync();

            response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task ListByUser_WithAuth_ShouldReturnOk()
        {
            var response = await _fixture.CreateAuthenticatedRequest("/network/listByUser")
                .AllowAnyHttpStatus()
                .GetAsync();

            response.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task ListByNetwork_AnonymousWithInvalidSlug_ShouldNotReturn401()
        {
            var response = await _fixture.CreateAnonymousRequest("/network/listByNetwork/non-existent-slug-999")
                .AllowAnyHttpStatus()
                .GetAsync();

            response.StatusCode.Should().NotBe(401, "endpoint is anonymous");
        }

        [Fact]
        public async Task ListByNetwork_Authenticated_ShouldReturnOkWithCreatorMember()
        {
            // The creator becomes an Active NetworkManager member on insert.
            var network = await CreateNetworkAsync();

            var response = await _fixture.CreateAuthenticatedRequest($"/network/listByNetwork/{network.Slug}")
                .AllowAnyHttpStatus()
                .GetAsync();

            response.StatusCode.Should().Be(200);
            var members = await response.GetJsonAsync<List<UserNetworkInfo>>();
            members.Should().NotBeNull();
            members.Should().Contain(m => m.NetworkId == network.NetworkId,
                "the authenticated caller must see the membership of the network they just created");
        }

        [Fact]
        public async Task ListByNetwork_Anonymous_ShouldReturnOnlyActiveMembers()
        {
            var network = await CreateNetworkAsync();

            var response = await _fixture.CreateAnonymousRequest($"/network/listByNetwork/{network.Slug}")
                .AllowAnyHttpStatus()
                .GetAsync();

            response.StatusCode.Should().Be(200);
            var members = await response.GetJsonAsync<List<UserNetworkInfo>>();
            members.Should().NotBeNull();
            // Anonymous callers must never see WaitForApproval/Inactive/Blocked members.
            members.Should().OnlyContain(m => m.Status == UserNetworkStatusEnum.Active,
                "anonymous (public storefront) callers only see Active members");
        }

        [Fact]
        public async Task GetById_WithoutAuth_ShouldReturn401()
        {
            var response = await _fixture.CreateAnonymousRequest("/network/getById/1")
                .AllowAnyHttpStatus()
                .GetAsync();

            response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task GetById_WithAuth_ShouldReturnSuccessOrNoContent()
        {
            var response = await _fixture.CreateAuthenticatedRequest("/network/getById/1")
                .AllowAnyHttpStatus()
                .GetAsync();

            response.StatusCode.Should().BeOneOf(200, 204);
        }

        [Fact]
        public async Task GetUserNetwork_WithoutAuth_ShouldReturn401()
        {
            var response = await _fixture.CreateAnonymousRequest("/network/getUserNetwork/1")
                .AllowAnyHttpStatus()
                .GetAsync();

            response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task GetUserNetwork_WithAuth_ShouldNotReturn401()
        {
            var response = await _fixture.CreateAuthenticatedRequest("/network/getUserNetwork/1")
                .AllowAnyHttpStatus()
                .GetAsync();

            response.StatusCode.Should().NotBe(401);
        }

        [Fact]
        public async Task GetUserNetworkBySlug_WithoutAuth_ShouldReturn401()
        {
            var response = await _fixture.CreateAnonymousRequest("/network/getUserNetworkBySlug/some-slug")
                .AllowAnyHttpStatus()
                .GetAsync();

            response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task GetUserNetworkBySlug_WithAuth_ShouldNotReturn401()
        {
            var response = await _fixture.CreateAuthenticatedRequest("/network/getUserNetworkBySlug/non-existent-slug-999")
                .AllowAnyHttpStatus()
                .GetAsync();

            response.StatusCode.Should().NotBe(401);
        }

        [Fact]
        public async Task GetSellerBySlug_AnonymousAllowed_ShouldNotReturn401()
        {
            var response = await _fixture.CreateAnonymousRequest("/network/getSellerBySlug/non-existent-network/non-existent-seller")
                .AllowAnyHttpStatus()
                .GetAsync();

            response.StatusCode.Should().NotBe(401, "endpoint is anonymous");
        }

        [Fact]
        public async Task GetSellerBySlug_WithNonExistentNetwork_ShouldReturnNetworkNotFound()
        {
            var response = await _fixture.CreateAnonymousRequest("/network/getSellerBySlug/non-existent-network-999/non-existent-seller-999")
                .AllowAnyHttpStatus()
                .GetAsync();

            response.StatusCode.Should().Be(500);
            var body = await response.GetJsonAsync<ErrorResult>();
            body.Sucesso.Should().BeFalse();
            body.MensagemErro.Should().Be("Network not found");
        }

        [Fact]
        public async Task GetSellerBySlug_WithExistingNetworkButMissingSeller_ShouldNotReturnNetworkNotFound()
        {
            var param = TestDataHelper.CreateNetworkInsertInfo();
            var insertResponse = await _fixture.CreateAuthenticatedRequest("/network/insert")
                .AllowAnyHttpStatus()
                .PostJsonAsync(param);
            insertResponse.StatusCode.Should().Be(200);
            var created = await insertResponse.GetJsonAsync<NetworkInfo>();
            created.Slug.Should().NotBeNullOrEmpty();

            var response = await _fixture.CreateAnonymousRequest($"/network/getSellerBySlug/{created.Slug}/non-existent-seller-999")
                .AllowAnyHttpStatus()
                .GetAsync();

            response.StatusCode.Should().Be(500, "seller lookup against NAuth fails when user does not exist");
            var body = await response.GetJsonAsync<ErrorResult>();
            body.Sucesso.Should().BeFalse();
            body.MensagemErro.Should().NotBe("Network not found", "network was just inserted with this slug — failure must come from seller lookup, not network lookup");
        }

        private class ErrorResult
        {
            [JsonPropertyName("sucesso")]
            public bool Sucesso { get; set; }

            [JsonPropertyName("mensagemErro")]
            public string MensagemErro { get; set; } = string.Empty;
        }

        [Fact]
        public async Task RequestAccess_WithoutAuth_ShouldReturn401()
        {
            var param = TestDataHelper.CreateNetworkRequestInfo();

            var response = await _fixture.CreateAnonymousRequest("/network/requestAccess")
                .AllowAnyHttpStatus()
                .PostJsonAsync(param);

            response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task RequestAccess_WithAuth_ShouldNotReturn401()
        {
            var param = TestDataHelper.CreateNetworkRequestInfo();

            var response = await _fixture.CreateAuthenticatedRequest("/network/requestAccess")
                .AllowAnyHttpStatus()
                .PostJsonAsync(param);

            response.StatusCode.Should().NotBe(401);
        }

        [Fact]
        public async Task ChangeStatus_WithoutAuth_ShouldReturn401()
        {
            var param = TestDataHelper.CreateNetworkChangeStatusInfo();

            var response = await _fixture.CreateAnonymousRequest("/network/changeStatus")
                .AllowAnyHttpStatus()
                .PostJsonAsync(param);

            response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task ChangeStatus_WithAuth_ShouldNotReturn401()
        {
            var param = TestDataHelper.CreateNetworkChangeStatusInfo();

            var response = await _fixture.CreateAuthenticatedRequest("/network/changeStatus")
                .AllowAnyHttpStatus()
                .PostJsonAsync(param);

            response.StatusCode.Should().NotBe(401);
        }

        [Fact]
        public async Task Promote_WithoutAuth_ShouldReturn401()
        {
            var response = await _fixture.CreateAnonymousRequest("/network/promote/1/1")
                .AllowAnyHttpStatus()
                .GetAsync();

            response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task Promote_WithAuth_ShouldNotReturn401()
        {
            var response = await _fixture.CreateAuthenticatedRequest("/network/promote/1/1")
                .AllowAnyHttpStatus()
                .GetAsync();

            response.StatusCode.Should().NotBe(401);
        }

        [Fact]
        public async Task Demote_WithoutAuth_ShouldReturn401()
        {
            var response = await _fixture.CreateAnonymousRequest("/network/demote/1/1")
                .AllowAnyHttpStatus()
                .GetAsync();

            response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task Demote_WithAuth_ShouldNotReturn401()
        {
            var response = await _fixture.CreateAuthenticatedRequest("/network/demote/1/1")
                .AllowAnyHttpStatus()
                .GetAsync();

            response.StatusCode.Should().NotBe(401);
        }

        // --- AbacatePay API key (served through MonexUp, relayed to ProxyPay) ---

        [Fact]
        public async Task SetAbacatePayApiKey_WithoutAuth_ShouldReturn401()
        {
            var response = await _fixture.CreateAnonymousRequest("/network/1/abacatepay-apikey")
                .AllowAnyHttpStatus()
                .PutJsonAsync(new AbacatePayApiKeyRequest { ApiKey = "abc_live_dummy" });

            response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task SetAbacatePayApiKey_WithAuth_EmptyKey_ShouldReturn400()
        {
            // Controller validates apiKey before reaching the provider — deterministic 400.
            var response = await _fixture.CreateAuthenticatedRequest("/network/1/abacatepay-apikey")
                .AllowAnyHttpStatus()
                .PutJsonAsync(new AbacatePayApiKeyRequest { ApiKey = string.Empty });

            response.StatusCode.Should().Be(400, "apiKey is required");
        }

        [Fact]
        public async Task SetAbacatePayApiKey_WithAuthAndProvisionedStore_ShouldNotReturn401Or500()
        {
            // Insert auto-provisions a ProxyPay store, so the key set reaches the provider.
            var network = await CreateNetworkAsync();

            var response = await _fixture.CreateAuthenticatedRequest($"/network/{network.NetworkId}/abacatepay-apikey")
                .AllowAnyHttpStatus()
                .PutJsonAsync(new AbacatePayApiKeyRequest { ApiKey = "abc_live_dummy_key" });

            // Provider may accept (204) or reject a dummy key (400) / non-owner (403);
            // it must never be an auth failure or an unhandled 500.
            ((int)response.StatusCode).Should().BeOneOf(204, 400, 403);
        }

        [Fact]
        public async Task GetAbacatePayApiKeyStatus_WithoutAuth_ShouldReturn401()
        {
            var response = await _fixture.CreateAnonymousRequest("/network/1/abacatepay-apikey/status")
                .AllowAnyHttpStatus()
                .GetAsync();

            response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task GetAbacatePayApiKeyStatus_WithAuth_ShouldReturnIndicatorShape()
        {
            var network = await CreateNetworkAsync();

            var response = await _fixture.CreateAuthenticatedRequest($"/network/{network.NetworkId}/abacatepay-apikey/status")
                .AllowAnyHttpStatus()
                .GetAsync();

            response.StatusCode.Should().NotBe(401);
            if ((int)response.StatusCode == 200)
            {
                var body = await response.GetJsonAsync<AbacatePayStatusResult>();
                body.Sucesso.Should().BeTrue();
                // hasAbacatePayApiKey is a bool indicator. Its value depends on the
                // caller's ProxyPay store state (one store per user, shared across
                // networks), so we assert the response shape, not a specific value.
            }
        }

        // --- Hierarchy (feature 010): GET /network/hierarchy/{networkId} ---

        [Fact]
        public async Task Hierarchy_WithoutAuth_ShouldReturn401()
        {
            var response = await _fixture.CreateAnonymousRequest("/network/hierarchy/1")
                .AllowAnyHttpStatus()
                .GetAsync();

            response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task Hierarchy_WithAuthAsMember_ShouldReturnOkAndShape()
        {
            // The creator becomes a member of the network on insert.
            var network = await CreateNetworkAsync();

            var response = await _fixture.CreateAuthenticatedRequest($"/network/hierarchy/{network.NetworkId}")
                .AllowAnyHttpStatus()
                .GetAsync();

            response.StatusCode.Should().Be(200, "the authenticated caller is a member of the network they just created");

            var hierarchy = await response.GetJsonAsync<HierarchyInfo>();
            hierarchy.Should().NotBeNull();
            hierarchy.NetworkId.Should().Be(network.NetworkId);
            hierarchy.Current.Should().NotBeNull("the logged-in member is the tree center");
            hierarchy.Current.UserId.Should().BeGreaterThan(0);
            hierarchy.Ancestors.Should().NotBeNull("ancestors must be a non-null collection (empty for the lone creator)");
            hierarchy.Descendants.Should().NotBeNull("descendants must be a non-null collection (empty for the lone creator)");
        }

        [Fact]
        public async Task Hierarchy_WithAuthNotMember_ShouldReturn404WithSucessoFalse()
        {
            // Caller does not belong to this networkId (very large, non-existent/foreign id).
            var response = await _fixture.CreateAuthenticatedRequest("/network/hierarchy/999999999")
                .AllowAnyHttpStatus()
                .GetAsync();

            response.StatusCode.Should().Be(404, "caller is not a member of the requested network");
            var body = await response.GetJsonAsync<ErrorResult>();
            body.Sucesso.Should().BeFalse();
        }

        // --- Invites (feature 012): pending no-account invites on /admin/teams ---

        [Fact]
        public async Task Invite_WithoutAuth_ShouldReturn401()
        {
            var param = TestDataHelper.CreateInviteRequestInfo();

            var response = await _fixture.CreateAnonymousRequest("/network/invite")
                .AllowAnyHttpStatus()
                .PostJsonAsync(param);

            response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task InviteList_WithoutAuth_ShouldReturn401()
        {
            var response = await _fixture.CreateAnonymousRequest("/network/invite/list/1")
                .AllowAnyHttpStatus()
                .GetAsync();

            response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task InviteCancel_WithoutAuth_ShouldReturn401()
        {
            var param = TestDataHelper.CreateInviteCancelInfo(1);

            var response = await _fixture.CreateAnonymousRequest("/network/invite/cancel")
                .AllowAnyHttpStatus()
                .PostJsonAsync(param);

            response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task InviteJoin_WithoutAuth_ShouldReturn401()
        {
            var param = TestDataHelper.CreateInviteActionInfo("any-token");

            var response = await _fixture.CreateAnonymousRequest("/network/invite/join")
                .AllowAnyHttpStatus()
                .PostJsonAsync(param);

            response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task InviteList_WithAuthAsManager_ShouldReturnOkAndEmptyForFreshNetwork()
        {
            var network = await CreateNetworkAsync();

            var response = await _fixture.CreateAuthenticatedRequest($"/network/invite/list/{network.NetworkId}")
                .AllowAnyHttpStatus()
                .GetAsync();

            response.StatusCode.Should().Be(200, "the creator is a NetworkManager of the network");
            var invites = await response.GetJsonAsync<List<NetworkInviteInfo>>();
            invites.Should().NotBeNull();
            invites.Should().BeEmpty("a freshly created network has no pending invites");
        }

        [Fact]
        public async Task InviteList_WithAuthNotManager_ShouldReturn403()
        {
            // Caller has no membership on this networkId → ValidateManager rejects.
            // This is the security constraint: pending invites carry invitee e-mails.
            var response = await _fixture.CreateAuthenticatedRequest("/network/invite/list/999999999")
                .AllowAnyHttpStatus()
                .GetAsync();

            response.StatusCode.Should().Be(403, "invitee e-mails must never reach a non-manager");
            var body = await response.GetJsonAsync<ErrorResult>();
            body.Sucesso.Should().BeFalse();
        }

        [Fact]
        public async Task Invite_NoAccountEmail_ShouldPersistPendingInviteAndAppearInList()
        {
            var network = await CreateNetworkAsync();
            var param = TestDataHelper.CreateInviteRequestInfo(network.NetworkId);

            var inviteResponse = await _fixture.CreateAuthenticatedRequest("/network/invite")
                .AllowAnyHttpStatus()
                .PostJsonAsync(param);

            inviteResponse.StatusCode.Should().Be(200);
            var result = await inviteResponse.GetJsonAsync<InviteResultInfo>();
            result.Sucesso.Should().BeTrue();
            result.HasAccount.Should().BeFalse("the address has no NAuth account");
            result.AlreadyMember.Should().BeFalse();
            result.Token.Should().NotBeNullOrEmpty();
            result.NetworkSlug.Should().Be(network.Slug);

            // The invite must now be visible to the manager on /admin/teams.
            var listResponse = await _fixture.CreateAuthenticatedRequest($"/network/invite/list/{network.NetworkId}")
                .AllowAnyHttpStatus()
                .GetAsync();
            listResponse.StatusCode.Should().Be(200);

            var invites = await listResponse.GetJsonAsync<List<NetworkInviteInfo>>();
            var invite = invites.Should().ContainSingle().Subject;
            invite.InviteId.Should().BeGreaterThan(0);
            invite.NetworkId.Should().Be(network.NetworkId);
            invite.Email.Should().Be(param.Email.ToLowerInvariant());
            invite.Status.Should().Be(NetworkInviteStatusEnum.Pending);
            invite.NetworkSlug.Should().Be(network.Slug);
            invite.CreatedAt.Should().NotBe(default);
            invite.InviterUserId.Should().Be(_fixture.ExtractUserIdFromToken());
            invite.Token.Should().Be(result.Token, "the listing re-signs the very same deterministic token");
        }

        [Fact]
        public async Task Invite_NoAccountEmail_TokenShouldCarryTheInviteIdSegment()
        {
            var network = await CreateNetworkAsync();
            var param = TestDataHelper.CreateInviteRequestInfo(network.NetworkId);

            var inviteResponse = await _fixture.CreateAuthenticatedRequest("/network/invite")
                .AllowAnyHttpStatus()
                .PostJsonAsync(param);
            inviteResponse.StatusCode.Should().Be(200);
            var result = await inviteResponse.GetJsonAsync<InviteResultInfo>();

            // Payload = networkId|inviterUserId|targetUserId|hasAccount|inviteId
            var segments = DecodeTokenPayload(result.Token).Split('|');
            segments.Should().HaveCount(5, "no-account invites carry the invite id as a 5th segment");
            segments[0].Should().Be(network.NetworkId.ToString());
            segments[2].Should().Be("0", "there is no target user id yet");
            segments[3].Should().Be("0", "hasAccount is false");
            long.Parse(segments[4]).Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Invite_NoAccountEmailTwice_ShouldReuseThePendingInvite()
        {
            var network = await CreateNetworkAsync();
            var param = TestDataHelper.CreateInviteRequestInfo(network.NetworkId);

            var first = await _fixture.CreateAuthenticatedRequest("/network/invite")
                .AllowAnyHttpStatus()
                .PostJsonAsync(param);
            first.StatusCode.Should().Be(200);
            var firstResult = await first.GetJsonAsync<InviteResultInfo>();

            var second = await _fixture.CreateAuthenticatedRequest("/network/invite")
                .AllowAnyHttpStatus()
                .PostJsonAsync(param);
            second.StatusCode.Should().Be(200);
            var secondResult = await second.GetJsonAsync<InviteResultInfo>();

            secondResult.Token.Should().Be(firstResult.Token, "the same pending invite is reused, so the link is stable");

            var listResponse = await _fixture.CreateAuthenticatedRequest($"/network/invite/list/{network.NetworkId}")
                .AllowAnyHttpStatus()
                .GetAsync();
            var invites = await listResponse.GetJsonAsync<List<NetworkInviteInfo>>();
            invites.Should().ContainSingle("re-inviting the same address must not duplicate the pending row");
        }

        [Fact]
        public async Task Invite_NoAccountEmail_ShouldStoreEmailLowercased()
        {
            var network = await CreateNetworkAsync();
            var mixedCase = $"MiXeD-{Guid.NewGuid().ToString("N")[..8]}@ApiTests.Invalid";
            var param = TestDataHelper.CreateInviteRequestInfo(network.NetworkId, mixedCase);

            var inviteResponse = await _fixture.CreateAuthenticatedRequest("/network/invite")
                .AllowAnyHttpStatus()
                .PostJsonAsync(param);
            inviteResponse.StatusCode.Should().Be(200);

            var listResponse = await _fixture.CreateAuthenticatedRequest($"/network/invite/list/{network.NetworkId}")
                .AllowAnyHttpStatus()
                .GetAsync();
            var invites = await listResponse.GetJsonAsync<List<NetworkInviteInfo>>();

            invites.Should().ContainSingle().Which.Email.Should().Be(mixedCase.ToLowerInvariant());
        }

        [Fact]
        public async Task Invite_WithInvalidEmail_ShouldReturn400()
        {
            var network = await CreateNetworkAsync();
            var param = TestDataHelper.CreateInviteRequestInfo(network.NetworkId, "not-an-email");

            var response = await _fixture.CreateAuthenticatedRequest("/network/invite")
                .AllowAnyHttpStatus()
                .PostJsonAsync(param);

            response.StatusCode.Should().Be(400);
            var result = await response.GetJsonAsync<InviteResultInfo>();
            result.Sucesso.Should().BeFalse();
        }

        [Fact]
        public async Task Invite_NonManagerCaller_ShouldNotCreateInvite()
        {
            // Caller is not a member of this network → ValidateManager rejects
            // before anything is persisted.
            var param = TestDataHelper.CreateInviteRequestInfo(999999999);

            var response = await _fixture.CreateAuthenticatedRequest("/network/invite")
                .AllowAnyHttpStatus()
                .PostJsonAsync(param);

            response.StatusCode.Should().NotBe(401, "the caller is authenticated");
            response.StatusCode.Should().NotBe(200, "a non-manager must not be able to invite");
        }

        [Fact]
        public async Task InviteCancel_WithAuthAsManager_ShouldRemoveInviteFromList()
        {
            var network = await CreateNetworkAsync();
            var invite = await CreatePendingInviteAsync(network);

            var cancelResponse = await _fixture.CreateAuthenticatedRequest("/network/invite/cancel")
                .AllowAnyHttpStatus()
                .PostJsonAsync(TestDataHelper.CreateInviteCancelInfo(invite.InviteId));

            cancelResponse.StatusCode.Should().Be(200);

            var listResponse = await _fixture.CreateAuthenticatedRequest($"/network/invite/list/{network.NetworkId}")
                .AllowAnyHttpStatus()
                .GetAsync();
            var invites = await listResponse.GetJsonAsync<List<NetworkInviteInfo>>();
            invites.Should().BeEmpty("a cancelled invite drops out of the pending listing");
        }

        [Fact]
        public async Task InviteCancel_Twice_ShouldFailOnTheSecondCall()
        {
            var network = await CreateNetworkAsync();
            var invite = await CreatePendingInviteAsync(network);
            var param = TestDataHelper.CreateInviteCancelInfo(invite.InviteId);

            var first = await _fixture.CreateAuthenticatedRequest("/network/invite/cancel")
                .AllowAnyHttpStatus()
                .PostJsonAsync(param);
            first.StatusCode.Should().Be(200);

            var second = await _fixture.CreateAuthenticatedRequest("/network/invite/cancel")
                .AllowAnyHttpStatus()
                .PostJsonAsync(param);

            second.StatusCode.Should().NotBe(200, "the invite is no longer pending");
        }

        [Fact]
        public async Task InviteCancel_WithUnknownInviteId_ShouldNotReturn200()
        {
            var response = await _fixture.CreateAuthenticatedRequest("/network/invite/cancel")
                .AllowAnyHttpStatus()
                .PostJsonAsync(TestDataHelper.CreateInviteCancelInfo(999999999));

            response.StatusCode.Should().NotBe(401, "the caller is authenticated");
            response.StatusCode.Should().NotBe(200, "the invite does not exist");
        }

        [Fact]
        public async Task InviteJoin_WithTamperedToken_ShouldNotReturn200()
        {
            var response = await _fixture.CreateAuthenticatedRequest("/network/invite/join")
                .AllowAnyHttpStatus()
                .PostJsonAsync(TestDataHelper.CreateInviteActionInfo("forged.token"));

            response.StatusCode.Should().NotBe(401, "the caller is authenticated");
            response.StatusCode.Should().NotBe(200, "a token that fails HMAC verification must be rejected");
        }

        [Fact]
        public async Task InviteJoin_WithPendingInviteToken_ShouldConsumeTheInvite()
        {
            // The caller is already an Active member (they created the network), so
            // the membership side is an idempotent no-op — what this asserts is the
            // 5th-segment reconciliation: the invite row must be marked Accepted and
            // disappear from the pending listing.
            var network = await CreateNetworkAsync();
            var invite = await CreatePendingInviteAsync(network);

            var joinResponse = await _fixture.CreateAuthenticatedRequest("/network/invite/join")
                .AllowAnyHttpStatus()
                .PostJsonAsync(TestDataHelper.CreateInviteActionInfo(invite.Token));

            joinResponse.StatusCode.Should().Be(200);

            var listResponse = await _fixture.CreateAuthenticatedRequest($"/network/invite/list/{network.NetworkId}")
                .AllowAnyHttpStatus()
                .GetAsync();
            var invites = await listResponse.GetJsonAsync<List<NetworkInviteInfo>>();
            invites.Should().NotContain(i => i.InviteId == invite.InviteId,
                "a consumed invite is Accepted and no longer pending");
        }

        [Fact]
        public async Task InviteJoin_Twice_ShouldBeIdempotent()
        {
            var network = await CreateNetworkAsync();
            var invite = await CreatePendingInviteAsync(network);
            var param = TestDataHelper.CreateInviteActionInfo(invite.Token);

            var first = await _fixture.CreateAuthenticatedRequest("/network/invite/join")
                .AllowAnyHttpStatus()
                .PostJsonAsync(param);
            first.StatusCode.Should().Be(200);

            var second = await _fixture.CreateAuthenticatedRequest("/network/invite/join")
                .AllowAnyHttpStatus()
                .PostJsonAsync(param);

            second.StatusCode.Should().Be(200, "joining with an already-consumed invite is a no-op, not an error");
        }

        [Fact]
        public async Task ListByNetwork_CreatorMembership_ShouldNotBeFlaggedAsInvited()
        {
            // Gap-B guard: a membership created by any path other than an invite
            // must report invited = false, so it never shows the "Convidado" badge.
            var network = await CreateNetworkAsync();

            var response = await _fixture.CreateAuthenticatedRequest($"/network/listByNetwork/{network.Slug}")
                .AllowAnyHttpStatus()
                .GetAsync();

            response.StatusCode.Should().Be(200);
            var members = await response.GetJsonAsync<List<UserNetworkInfo>>();
            members.Should().OnlyContain(m => m.Invited == false,
                "the network creator was not invited");
        }

        /// <summary>Creates a network-scoped pending invite and returns its listed row.</summary>
        private async Task<NetworkInviteInfo> CreatePendingInviteAsync(NetworkInfo network)
        {
            var param = TestDataHelper.CreateInviteRequestInfo(network.NetworkId);
            var response = await _fixture.CreateAuthenticatedRequest("/network/invite")
                .AllowAnyHttpStatus()
                .PostJsonAsync(param);
            response.StatusCode.Should().Be(200, "the caller is a manager and the address has no account");

            var listResponse = await _fixture.CreateAuthenticatedRequest($"/network/invite/list/{network.NetworkId}")
                .AllowAnyHttpStatus()
                .GetAsync();
            listResponse.StatusCode.Should().Be(200);

            var invites = await listResponse.GetJsonAsync<List<NetworkInviteInfo>>();
            return invites.Single(i => i.Email == param.Email.ToLowerInvariant());
        }

        private static string DecodeTokenPayload(string token)
        {
            var payload = token.Split('.')[0].Replace('-', '+').Replace('_', '/');
            switch (payload.Length % 4)
            {
                case 2: payload += "=="; break;
                case 3: payload += "="; break;
            }
            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload));
        }

        private async Task<NetworkInfo> CreateNetworkAsync()
        {
            var payload = TestDataHelper.CreateNetworkInsertInfo();
            var response = await _fixture.CreateAuthenticatedRequest("/network/insert")
                .AllowAnyHttpStatus()
                .PostJsonAsync(payload);

            response.StatusCode.Should().Be(200, "network must be created (auto-provisions the ProxyPay store)");
            return await response.GetJsonAsync<NetworkInfo>();
        }

        private class AbacatePayStatusResult
        {
            [JsonPropertyName("sucesso")]
            public bool Sucesso { get; set; }

            [JsonPropertyName("hasAbacatePayApiKey")]
            public bool HasAbacatePayApiKey { get; set; }
        }
    }
}
