import urllib.request
import re

url = "https://github.com/IT24104023/ClearToWork/actions"
req = urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0'})
html = urllib.request.urlopen(req).read().decode('utf-8')

runs = re.findall(r'href="/IT24104023/ClearToWork/actions/runs/(\d+)"', html)
run_ids = list(dict.fromkeys(runs))[:5]
print("Latest run IDs on Actions page:", run_ids)

for r in run_ids[:1]:
    run_url = f"https://github.com/IT24104023/ClearToWork/actions/runs/{r}"
    run_html = urllib.request.urlopen(urllib.request.Request(run_url, headers={'User-Agent': 'Mozilla/5.0'})).read().decode('utf-8')
    
    # Check for failure vs success
    if "This run failed" in run_html or "failed in" in run_html:
        print(f"Run {r}: STATUS = FAILED")
    elif "completed successfully" in run_html or "all checks have passed" in run_html:
        print(f"Run {r}: STATUS = SUCCESS (GREEN)")
    else:
        print(f"Run {r}: STATUS = IN_PROGRESS")
