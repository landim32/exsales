import { Copy, Mail, X } from "lucide-react";

import UserNetworkSearchInfo from "../../DTO/Domain/UserNetworkSearchInfo";
import { ActionButton } from "./UserSearchRow";

export interface InviteSearchRowLabels {
  /** Status pill text, e.g. "Convite pendente". */
  statusText: string;
  /** Badge text, e.g. "Convidado". */
  invitedBadge: string;
  copyLink: string;
  cancelInvite: string;
  /** "Convidado por {{name}}" — already interpolated by the parent. */
  invitedBy?: string;
}

export interface InviteRowHandlers {
  onCopyLink: (invite: UserNetworkSearchInfo) => void;
  onCancel: (invite: UserNetworkSearchInfo) => void;
}

export interface InviteSearchRowProps {
  invite: UserNetworkSearchInfo;
  labels: InviteSearchRowLabels;
  handlers: InviteRowHandlers;
}

/**
 * InviteSearchRow — a pending invite sent to an e-mail with no account yet,
 * rendered inside the same `/admin/teams` list as regular members.
 *
 * It deliberately does NOT reuse UserSearchRow: an invite has no user id, no
 * profile and no role, so the profile/role chips and the whole member action
 * cluster (promote/demote/approve/block) are meaningless here. Only the two
 * layouts and the visual language are shared.
 *
 * Pure presentational — the parent owns the refresh after each action.
 */

/** Initials from an e-mail address, since there is no name yet. */
function emailInitials(email: string | undefined | null): string {
  if (!email) return "—";
  const local = email.split("@")[0];
  if (!local) return "—";
  return local.slice(0, 2).toUpperCase();
}

function ActionCluster({
  invite,
  labels,
  handlers,
}: {
  invite: UserNetworkSearchInfo;
  labels: InviteSearchRowLabels;
  handlers: InviteRowHandlers;
}) {
  return (
    <>
      <ActionButton
        ariaLabel={labels.copyLink}
        onClick={() => handlers.onCopyLink(invite)}
        tone="primary"
      >
        <Copy size={16} aria-hidden="true" />
      </ActionButton>
      <ActionButton
        ariaLabel={labels.cancelInvite}
        onClick={() => handlers.onCancel(invite)}
        tone="danger"
      >
        <X size={16} aria-hidden="true" />
      </ActionButton>
    </>
  );
}

export default function InviteSearchRow({
  invite,
  labels,
  handlers,
}: InviteSearchRowProps) {
  const initials = emailInitials(invite.email);

  return (
    <>
      {/* Desktop / tablet — grid row ------------------------------------ */}
      <div
        className="hidden md:!grid grid-cols-12 items-center gap-4 px-4 h-14 border-b border-mnx-neutral-100 last:border-b-0 hover:bg-orange-500/5 transition-colors duration-fast"
        role="row"
      >
        {/* Invitee cell — the e-mail is the primary line, there is no name yet */}
        <div className="col-span-4 min-w-0 flex items-center gap-3" role="cell">
          <span
            aria-hidden="true"
            className="inline-flex w-8 h-8 items-center justify-center rounded-full bg-amber-500/10 text-amber-700 ring-1 ring-amber-500/20 text-[0.7rem] font-bold tabular-nums shrink-0"
          >
            {initials}
          </span>
          <div className="min-w-0">
            <div className="text-sm font-semibold text-graphite-900 truncate flex items-center gap-1.5">
              <Mail size={13} aria-hidden="true" className="text-graphite-400 shrink-0" />
              {invite.email}
            </div>
            {labels.invitedBy && (
              <div className="text-xs text-graphite-500 truncate">
                {labels.invitedBy}
              </div>
            )}
          </div>
        </div>

        {/* No profile / no role for an invite — the cells stay empty on purpose */}
        <div className="col-span-2" role="cell" />
        <div className="col-span-2" role="cell" />

        {/* Status pill */}
        <div className="col-span-2 flex items-center justify-end gap-1" role="cell">
          <span className="inline-flex items-center h-[26px] px-2 rounded-full bg-amber-500/10 text-amber-700 ring-1 ring-amber-500/20 text-xs font-semibold">
            {labels.statusText}
          </span>
        </div>

        {/* Actions */}
        <div className="col-span-2 flex items-center justify-end gap-1" role="cell">
          <ActionCluster invite={invite} labels={labels} handlers={handlers} />
        </div>
      </div>

      {/* Mobile — stacked card ----------------------------------------- */}
      <div className="md:hidden border-b border-mnx-neutral-100 last:border-b-0 px-4 py-4 hover:bg-orange-500/5 transition-colors duration-fast">
        <div className="flex items-start justify-between gap-3">
          <div className="flex items-start gap-3 min-w-0 flex-1">
            <span
              aria-hidden="true"
              className="inline-flex w-9 h-9 items-center justify-center rounded-full bg-amber-500/10 text-amber-700 ring-1 ring-amber-500/20 text-xs font-bold tabular-nums shrink-0"
            >
              {initials}
            </span>
            <div className="min-w-0">
              <div className="text-sm font-semibold text-graphite-900 truncate">
                {invite.email}
              </div>
              {labels.invitedBy && (
                <div className="text-xs text-graphite-500 truncate">
                  {labels.invitedBy}
                </div>
              )}
            </div>
          </div>
          <div className="flex items-center gap-1 shrink-0">
            <ActionCluster invite={invite} labels={labels} handlers={handlers} />
          </div>
        </div>
        <div className="mt-3 flex flex-wrap items-center gap-2">
          <span className="inline-flex items-center h-[24px] px-2 rounded-full bg-amber-500/10 text-amber-700 ring-1 ring-amber-500/20 text-[11px] font-semibold">
            {labels.statusText}
          </span>
        </div>
      </div>
    </>
  );
}
